using UnityEngine;
namespace Commongame
{
    public class Controlls : MonoBehaviour
    {

        private bool takeInput = false;
        [SerializeField] private LayerMask clickableMask;

        public void Init()
        {
            GameManager.Instance._events.LevelStarted.AddListener(ResumeInput);
            StopInput();
        }

        public void StopInput()
        {
            takeInput = false;
        }
        public void ResumeInput()
        {
            takeInput = true;
        }
        float lastXpos = 0;
        float currentXpos = 0;
        private void Update()
        {
            if (takeInput)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    lastXpos = currentXpos;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    lastXpos = currentXpos;
                    
                    GameManager.Instance._events.Input.Invoke(0);
                }
                if (Input.GetMouseButton(0))
                {
                    currentXpos = MouseScreenPosition().x;
                    if((currentXpos - lastXpos) > 0)
                    {
                        GameManager.Instance._events.Input.Invoke(1);
                    }
                    else if((currentXpos - lastXpos) < 0)
                    {
                        GameManager.Instance._events.Input.Invoke(-1);
                    } else if ((currentXpos - lastXpos) == 0)
                    {
                        GameManager.Instance._events.Input.Invoke(0);
                    }
                }

                lastXpos = currentXpos;
            }
        }

        private void SendInpEvent()
        {
            float x = MouseScreenPosition().x - Screen.width/2;
            if (x >= 0)
                GameManager.Instance._events.Input.Invoke(1);
            else
                GameManager.Instance._events.Input.Invoke(-1);

        }

        public Vector3 MouseScreenPosition()
        {
            return Input.mousePosition;
        }
    }
}