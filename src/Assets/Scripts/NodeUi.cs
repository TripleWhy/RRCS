namespace AssemblyCSharp
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using UnityEngine.UI.Extensions;
    using System;

    public abstract class NodeUi : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,
        IBoxSelectable
    {
        public RectTransform selectionPrefab;

        private CircuitNode node;

        protected PortUi[] inPorts;
        protected PortUi[] outPorts;
        protected PortUi[] statePorts;

        private Vector3 pointerWorldOffset;
        protected RectTransform rectTransform;
        private Canvas canvas;
        private RectTransform canvasRectTransform;
        private NodeUi draggingInstance;
        private RectTransform selectionInstance;
        private bool isSidebarNode = true;

        protected void Awake()
        {
            rectTransform = (RectTransform) transform;
            canvas = GetComponentInParent<Canvas>();
            canvasRectTransform = (RectTransform) canvas.transform;
            isSidebarNode = !ReferenceEquals(canvas, RRCSManager.Instance.WorldCanvas);

            CreateNode();
        }

        protected void CreateNode()
        {
            Node = CreateNode(IsSidebarNode ? null : RRCSManager.Instance.circuitManager);
        }

        protected abstract CircuitNode CreateNode(CircuitManager manager);

        void OnDestroy()
        {
            UiManager.Unregister(this);
            if (Node != null)
                Node.Destroy();
        }

        public CircuitNode Node
        {
            get { return node; }
            protected set
            {
                if (node != null)
                    throw new InvalidOperationException();
                if (value == null)
                    return;
                node = value;
                if (!IsSidebarNode)
                    OnMovedToWorld();
                UiManager.Register(this);
            }
        }

        public int InPortCount
        {
            get { return Node.inputPortCount; }
        }

        public int TotalInPortCount
        {
            get { return Node.inputPorts.Length; }
        }

        public int OutPortCount
        {
            get { return Node.outputPortCount; }
        }

        public int TotalOutPortCount
        {
            get { return Node.outputPorts.Length; }
        }

        public int TotalStatePortCount
        {
            get { return Node.statePort != null ? 1 : 0; }
        }

        public bool HasReset
        {
            get { return Node.hasReset; }
        }

        public bool IsSidebarNode
        {
            get { return isSidebarNode; }
            private set
            {
                if (value == isSidebarNode)
                    return;
                if (!isSidebarNode && value)
                    throw new InvalidOperationException();
                if (!isSidebarNode && Node == null)
                    throw new InvalidOperationException();
                isSidebarNode = value;
                EnableRaycast(true);
                OnMovedToWorld();

                Debug.Assert(inPorts != null);
                Debug.Assert(inPorts.Length == Node.inputPorts.Length);
                Debug.Assert(outPorts != null);
                Debug.Assert(outPorts.Length == Node.outputPorts.Length);
                for (int i = 0; i < inPorts.Length; i++)
                {
                    inPorts[i].Port = Node.inputPorts[i];
                    UiManager.Register(inPorts[i]);
                }

                for (int i = 0; i < outPorts.Length; i++)
                {
                    outPorts[i].Port = Node.outputPorts[i];
                    UiManager.Register(outPorts[i]);
                }

                for (int i = 0; i < statePorts.Length; i++)
                {
                    statePorts[i].Port = Node.statePort;
                    UiManager.Register(statePorts[i]);
                }

                Node.Manager = RRCSManager.Instance.circuitManager;
                UiManager.Register(this);
            }
        }

        protected virtual void EnableRaycast(bool on)
        {
            GetComponent<Image>().raycastTarget = on;
        }

        protected virtual void OnMovedToWorld()
        {
        }

        public virtual string GetParams()
        {
            return "";
        }

        public virtual void ParseParams(string parameters)
        {
        }

        #region IPointerDownHandler implementation

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != 0)
                return;
            DoPointerDown(eventData);
            foreach (NodeUi node in RRCSManager.Instance.selectionManager.GetSelectedNodes())
                if (!object.ReferenceEquals(node, this))
                    node.DoPointerDown(eventData);
        }

        public void DoPointerDown(PointerEventData eventData)
        {
            Vector2 n_originalLocalPointerPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position,
                eventData.pressEventCamera, out n_originalLocalPointerPosition);
            pointerWorldOffset = rectTransform.InverseTransformPoint(n_originalLocalPointerPosition) -
                                 rectTransform.InverseTransformPoint(new Vector3());
        }

        #endregion

        #region IBeginDragHandler implementation

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != 0)
                return;
            if (draggingInstance != null)
                return;
            if (IsSidebarNode)
            {
                draggingInstance = Instantiate(this, canvasRectTransform);
                draggingInstance.EnableRaycast(false);
            }

            RRCSManager.Instance.selectionManager.SelectionEnabled = false;
        }

        #endregion

        #region IDragHandler implementation

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != 0)
                return;
            DoDrag(eventData);
            foreach (NodeUi node in RRCSManager.Instance.selectionManager.GetSelectedNodes())
                if (!object.ReferenceEquals(node, this))
                    node.DoDrag(eventData);
        }

        private void DoDrag(PointerEventData eventData)
        {
            NodeUi node = draggingInstance ?? this;

            Debug.Assert(node.rectTransform != null);
            Debug.Assert(node.canvasRectTransform != null);

            Vector2 worldPosition = eventData.position;
            if (eventData.pressEventCamera != null)
                worldPosition = eventData.pressEventCamera.ScreenToWorldPoint(eventData.position);

            node.rectTransform.position = (Vector3) (worldPosition) - pointerWorldOffset;
        }

        #endregion

        #region IEndDragHandler implementation

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != 0)
                return;
            RRCSManager.Instance.selectionManager.SelectionEnabled = true;
            if (draggingInstance == null)
                return;

            Canvas worldCanvas = RRCSManager.Instance.WorldCanvas;
            bool isWorldPos = (eventData.hovered.Count == 0);
            if (!isWorldPos)
            {
                print("Drop world: " + worldCanvas.gameObject);
                foreach (GameObject h in eventData.hovered)
                {
                    print("Drop hover: " + h);
                    if (object.ReferenceEquals(h, worldCanvas.gameObject))
                    {
                        isWorldPos = true;
                        break;
                    }
                }
            }

            if (!isWorldPos)
            {
                Destroy(draggingInstance.gameObject);
            }
            else
            {
                Vector2 newPos = worldCanvas.worldCamera.ScreenToWorldPoint(draggingInstance.rectTransform.position);
                draggingInstance.rectTransform.SetParent(worldCanvas.transform, false);
                draggingInstance.rectTransform.position = newPos;
                draggingInstance.canvas = worldCanvas;
                draggingInstance.canvasRectTransform = (RectTransform) worldCanvas.transform;
                draggingInstance.IsSidebarNode = false;
            }

            draggingInstance = null;
        }

        #endregion

        #region Implemented members of IBoxSelectable

        bool _selected = false;

        public bool selected
        {
            get { return _selected; }
            set
            {
                Debug.Assert(!IsSidebarNode);
                if (value == _selected)
                    return;
                _selected = value;
                UpdateSelected();
            }
        }

        bool _preSelected = false;

        public bool preSelected
        {
            get { return _preSelected; }
            set
            {
                if (IsSidebarNode)
                    return;
                if (value == _preSelected)
                    return;
                _preSelected = value;
                UpdateSelected();
            }
        }

        #endregion

        private void UpdateSelected()
        {
            if ((preSelected || selected) && selectionInstance == null)
            {
                selectionInstance = Instantiate(selectionPrefab, rectTransform);
                UpdateSelectionSize();
                RRCSManager.Instance.cameraControls.ZoomChanged += Camera_ZoomChanged;
            }
            else if (!preSelected && !selected && selectionInstance != null)
            {
                Destroy(selectionInstance.gameObject);
                RRCSManager.Instance.cameraControls.ZoomChanged -= Camera_ZoomChanged;
                selectionInstance = null;
            }
        }

        private void Camera_ZoomChanged(float inverseZoom)
        {
            Vector2 pos = new Vector2(0, 0);
            Vector2 size = rectTransform.sizeDelta;
            if (inPorts.Length != 0)
            {
                if (outPorts.Length != 0)
                    size.x += inPorts[0].RectTransform.sizeDelta.x;
                else
                {
                    size.x += inPorts[0].RectTransform.sizeDelta.x * 0.5f;
                    pos.x -= inPorts[0].RectTransform.sizeDelta.x * 0.25f;
                }
            }
            else
            {
                if (outPorts.Length != 0)
                {
                    size.x += outPorts[0].RectTransform.sizeDelta.x * 0.5f;
                    pos.x += outPorts[0].RectTransform.sizeDelta.x * 0.25f;
                }
            }

            if (selectionInstance != null)
            {
                selectionInstance.anchoredPosition = pos;
                selectionInstance.sizeDelta = size / inverseZoom;
                selectionInstance.localScale = new Vector3(inverseZoom, inverseZoom);
            }
        }

        private void UpdateSelectionSize()
        {
            Camera_ZoomChanged(RRCSManager.Instance.cameraControls.InverseZoom);
        }
    }
}