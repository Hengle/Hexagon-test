using Svelto.ECS;
using UnityEngine;

namespace Hexagon.ECS.Unit
{
    public class UnitMovementImplementor : MonoBehaviour, IImplementor, ITransformComponent, IUnitAnimationComponent, ITurnComponent,
        IUnitMovementComponent
    {
        private bool _isWalking;
        private Animator anim;
        private Transform unitTransform;

        public Vector3 position
        {
            get { return unitTransform.position; }
            set { unitTransform.position = value; }
        }

        public Vector3 rotation
        {
            get { return unitTransform.localEulerAngles; }
            set { unitTransform.localEulerAngles = value; }
        }

        public Quaternion localRotation
        {
            get { return unitTransform.localRotation; }
            set { unitTransform.localRotation = value; }
        }

        public void setBool(string name, bool value)
        {
            anim.SetBool(name, value);
        }

        public string trigger
        {
            set { anim.SetTrigger(value); }
        }

        public bool isWalking
        {
            get { return _isWalking; }
            set
            {
                _isWalking = value;
                setBool("IsWalking", value);
            }
        }

        public int move(Vector3 target)
        {
            var id = LeanTween.move(gameObject, target, 0.5f).setEase(LeanTweenType.linear).id;
            return id;
        }

        public int rotate(Vector3 localEuler)
        {
            var id = LeanTween.rotate(gameObject, localEuler, 0.25f).setEase(LeanTweenType.easeInOutQuad).id;
            return id;
        }

        public int maxMove { get; set; }

        private void Awake()
        {
            unitTransform = transform;
            anim = GetComponent<Animator>();
            isMyTurn = new DispatchOnSet<bool>(gameObject.GetInstanceID());
            currentAbility = new DispatchOnSet<int>(gameObject.GetInstanceID());
            maxMove = 5;
            moveCounter = new DispatchOnChange<int>(gameObject.GetInstanceID());
            state = new DispatchOnSet<UnitTurnState>(gameObject.GetInstanceID());
        }

        public int ctrCounter { get; set; }
        public DispatchOnChange<int> moveCounter { get; set; }
        public bool isInBattle { get; set; }
        public DispatchOnSet<bool> isMyTurn { get; private set; }
        public DispatchOnSet<UnitTurnState> state { get; set; }
        public DispatchOnSet<int> currentAbility { get; private set; }
    }
}