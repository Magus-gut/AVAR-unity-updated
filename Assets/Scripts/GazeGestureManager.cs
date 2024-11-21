using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;
// New in AVAR-Updated
using UnityEngine.XR.Interaction.Toolkit;



public class GazeGestureManager : MonoBehaviour
{
    public static GazeGestureManager Instance { get; private set; }

    // Represents the hologram that is currently being gazed at.
    public XRRayInteractor rayInteractor;
    public GameObject FocusedObject { get; private set; }
    //private GameObject SelectedObject;
    //private GameObject originObject;
    
    
    private LineRenderer line;
    private NavigationReactor m_reactor;
    private GameObject originObject;
    private InteractiveGameObject objectHolded;
    
    //Esto es útil para rotacion
    private Vector3 initialHandPosition;
    private Quaternion initialObjectRotation;

    
    // Start is called before the first frame update
    void Awake()
    {
        
        Instance = this;

        line = gameObject.AddComponent<LineRenderer>();

        line.startColor = Color.green;
        line.endColor = Color.blue;

        line.startWidth = 0;
        line.endWidth = 0;

        line.enabled = false;

        Debug.Log("GazeGestureManager Awake!");
        // dejar esto?
        playground.Instance.f_Init();
    }
    
    private void OnEnable()
    {
        // Tap & Hold listen
        rayInteractor.selectEntered.AddListener(OnTapGesture);
        rayInteractor.selectExited.AddListener(OnHoldGestureEnd);
    }
    
    private void OnDisable()
    {
        // Tap & Hold un-listen
        rayInteractor.selectEntered.RemoveListener(OnTapGesture);
        rayInteractor.selectExited.RemoveListener(OnHoldGestureEnd);
    }
    
    private void OnTapGesture(SelectEnterEventArgs args)
    {
        // Tap sim
        FocusedObject = args.interactableObject.transform.gameObject;
        originObject = FocusedObject;

        if (objectHolded != null)
        {
            objectHolded.setInactive();
            objectHolded = null;
        }
        else
        {
            if (FocusedObject != null)
            {
                objectHolded = FocusedObject.GetComponent<InteractiveGameObject>();
                if (objectHolded != null)
                {
                    objectHolded.OnAirTapped(rayInteractor.transform.position); // Tap logic
                }
                
                // Rotation?
                initialHandPosition = rayInteractor.transform.position;
                initialObjectRotation = FocusedObject.transform.rotation;
            }
        }

        // Enable Line renderer for selection
        line.enabled = true;
    }
    
    private void OnHoldGestureEnd(SelectExitEventArgs args)
    {
        // Hold end
        line.enabled = false;
        FocusedObject = null;
        objectHolded = null;
    }
    
    

    // Update is called once per frame
    private void Update()
    {
        if (FocusedObject != null)
        {
            // Update Line renderer pos
            line.SetPosition(0, rayInteractor.transform.position);
            line.SetPosition(1, FocusedObject.transform.position);

            // Hold selected objetct
            if (objectHolded != null && rayInteractor.isSelectActive)
            {
                // Displacement from hand vector
                Vector3 currentHandPosition = rayInteractor.transform.position;
                Vector3 displacement = currentHandPosition - initialHandPosition;

                // Apply displacement as rotation
                float rotationSpeed = 50.0f; // Ajusta la velocidad de rotación
                Quaternion rotationChange = Quaternion.Euler(displacement.y * rotationSpeed, -displacement.x * rotationSpeed, 0);
                FocusedObject.transform.rotation = initialObjectRotation * rotationChange;

                // Aquí puedes agregar lógica adicional si es necesario
                objectHolded.OnHold(rayInteractor.transform.position);
            }
        }
    }

    private void showPopup()
    {
        if (FocusedObject == null)
        {
            playground.Instance.PopupPanel.m_text.text = "";
            playground.Instance.PopupPanel.gameObject.SetActive(false);
        }
        else
        {
            if (FocusedObject.GetComponent<InteractiveGameObject>() == null)
            {
                playground.Instance.PopupPanel.m_text.text = "";
                playground.Instance.PopupPanel.gameObject.SetActive(false);
            }
            else
            {
                var msg = FocusedObject.GetComponent<InteractiveGameObject>().popup_msg;
                playground.Instance.PopupPanel.m_text.text = msg;
                playground.Instance.PopupPanel.gameObject.SetActive(msg != "");
            }
        }
    }

}
