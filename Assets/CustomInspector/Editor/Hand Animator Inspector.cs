using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

//[CustomEditor(typeof(HandAnimator))]
public class HandAnimatorInspector : Editor
{
    public VisualTreeAsset m_InspectorXML;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement myInspector = new VisualElement();

        myInspector.Add(new Label("This is a custom inspector"));

        m_InspectorXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/CustomInspector/HandAnimatorInspectorUXML.uxml");

        myInspector = m_InspectorXML.Instantiate();

        return myInspector;
    }
}
