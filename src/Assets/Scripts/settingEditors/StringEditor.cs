namespace AssemblyCSharp
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class StringEditor : MonoBehaviour
    {
        [System.Serializable]
        public class ValueChangeEvent : UnityEvent<string>
        {
        }

        public delegate void ValueChangedEventHandler(StringEditor sender, string value);

        public event ValueChangedEventHandler ValueChanged = delegate { };
        public ValueChangeEvent onValueChanged;

        [SerializeField]
        private string currentValue = "";
        private NodeSetting setting;
        private Text settingNameText;
        private InputField input;

        void Awake()
        {
            foreach (Transform child in transform)
            {
                if (settingNameText == null)
                    settingNameText = child.GetComponent<Text>();
                else if (input == null)
                    input = child.GetComponent<InputField>();
            }

            DebugUtils.Assert(input != null);

            input.text = Value;

            input.onValueChanged.AddListener(OnInputChanged);
        }

        void Start()
        {
            if (onValueChanged == null)
                onValueChanged = new ValueChangeEvent();
        }

        private void OnInputChanged(string text)
        {
            Value = text;
            ValueChanged(this, text);
            onValueChanged.Invoke(text);
        }

        
        public string Value
        {
            get { return currentValue; }
            set
            {
                currentValue = value;
                input.text = value;
            }
        }

        public NodeSetting Setting
        {
            get { return setting; }
            set
            {
                settingNameText.text = value.displayName;
                Value = (string) value.currentValue;
                setting = value;
            }
        }
    }
}