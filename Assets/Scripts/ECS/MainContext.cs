using Hexagon.ECS.Map;
using Hexagon.ECS.Battle;
using Svelto.Context;
using Svelto.ECS;
using Svelto.ECS.Schedulers.Unity;
using System.Collections.Generic;
using Hexagon.ECS.Ability;
using UnityEngine;
using Svelto.Tasks;
using Hexagon.ECS.Engines.Unit;
using Hexagon.ECS.Player;
using Hexagon.ECS.Camera;
using Hexagon.ECS.Others;
using Hexagon.ECS.Unit;
using Hexagon.Services.Map;
using Hexagon.Services.UI;

namespace Hexagon.ECS {

    public class Main : ICompositionRoot {

        public Main() {
            SetupEnginesAndEntities();
        }

        void SetupEnginesAndEntities() {
            _enginesRoot = new EnginesRoot(new UnitySumbmissionEntityViewScheduler());
            _entityFactory = _enginesRoot.GenerateEntityFactory();
            _prefabsDictionary = new PrefabsDictionary(Application.persistentDataPath + "/prefabs.json");
            var entityFunctions = _enginesRoot.GenerateEntityFunctions();

            //GameObjectFactory factory = new GameObjectFactory();

            _mapManager = GameObject.FindGameObjectWithTag("HexGrid").GetComponent<HexGrid>();

            _uiService = _prefabsDictionary.Istantiate("UI Service").GetComponent<UIService>();

            Sequencer battleTurnSequence = new Sequencer();
            Sequencer abilityUsedSequence = new Sequencer();

            ITime time = new Time();

            MapGeneratorEngine mapGeneratorEngine = new MapGeneratorEngine(_mapManager, _entityFactory, _prefabsDictionary);
            UnitSpawnerEngine unitSpawnerEngine = new UnitSpawnerEngine(_prefabsDictionary, _entityFactory, _mapManager);
            PlayerInputEngine playerInputEngine = new PlayerInputEngine(_mapManager);
            BattleInitEngine battleInitEngine = new BattleInitEngine(entityFunctions, _prefabsDictionary, _entityFactory);
            TurnOrderEngine turnOrderEngine = new TurnOrderEngine(battleTurnSequence, _uiService);
            UnitTurnActionEngine unitTurnActionEngine = new UnitTurnActionEngine(battleTurnSequence);
            UnitMovementEngine unitMovementEngine = new UnitMovementEngine(_mapManager);
            UnitHighlightEngine unitHighlightEngine = new UnitHighlightEngine(_mapManager);
            BattleHudEngine battleHudEngine = new BattleHudEngine(_uiService);
            HealthEngine healthEngine = new HealthEngine();

            AbilitySelectionEngine abilitySelectionEngine = new AbilitySelectionEngine(_uiService, abilityUsedSequence);
            AbilityTargetEngine abilityTargetEngine = new AbilityTargetEngine();


            battleTurnSequence.SetSequence(
                new Steps 
                {
                    {
                        turnOrderEngine,
                        new To
                        {
                            unitTurnActionEngine,
                        }
                    },
                    {
                        unitTurnActionEngine,
                        new To
                        {
                            { TurnCondition.starting, new IStep[] { playerInputEngine, battleHudEngine } },
                            { TurnCondition.ending, new IStep[] { playerInputEngine, battleHudEngine, turnOrderEngine} }
                        }
                    },

                }
            );

            abilityUsedSequence.SetSequence(
                new Steps 
                {
                    {
                        abilitySelectionEngine,
                        new To
                        {
                            { new IStep[]  { abilityTargetEngine, unitHighlightEngine } }
                        }
                    },
                }
            );

            AddEngine(mapGeneratorEngine);
            AddEngine(unitSpawnerEngine);
            AddEngine(playerInputEngine);
            AddEngine(battleInitEngine);
            AddEngine(turnOrderEngine);
            AddEngine(unitTurnActionEngine);
            AddEngine(unitMovementEngine);
            AddEngine(unitHighlightEngine);
            AddEngine(new CameraFollowTargetEngine(time));
            AddEngine(battleHudEngine);
            AddEngine(healthEngine);

            AddEngine(abilitySelectionEngine);
            AddEngine(abilityTargetEngine);
        }

        void AddEngine(IEngine engine) {
            _enginesRoot.AddEngine(engine);
        }

        void ICompositionRoot.OnContextCreated(UnityContext contextHolder) {
            //BuildCameraEntity();
        }

        void BuildCameraEntity() {
            var implementor = UnityEngine.Camera.main.gameObject.AddComponent<CameraImplementor>();
            _entityFactory.BuildEntity<CameraEntityDescriptor>(UnityEngine.Camera.main.GetInstanceID(), new object[] { implementor });
        }


        void ICompositionRoot.OnContextDestroyed() {
            _enginesRoot.Dispose();
            TaskRunner.StopAndCleanupAllDefaultSchedulers();
        }

        void ICompositionRoot.OnContextInitialized() {            
        }

        EnginesRoot _enginesRoot;
        PrefabsDictionary _prefabsDictionary;
        IEntityFactory _entityFactory;
        private HexGrid _mapManager;
        private UIService _uiService;
    }

    public class MainContext : UnityContext<Main> {

    }
}
