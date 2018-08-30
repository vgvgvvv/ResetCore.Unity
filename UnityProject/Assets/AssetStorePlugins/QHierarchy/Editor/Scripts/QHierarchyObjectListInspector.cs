using UnityEditor;

namespace qtools.qhierarchy
{
    [CustomEditor(typeof(QObjectList))]
    public class QHierarchyObjectListInspector : Editor
    {
    	public override void OnInspectorGUI()
    	{
    		EditorGUILayout.HelpBox("\nThis is an auto created GameObject that managed by QHierarchy.\n\n" + 
                                    "It stores references to 'Locked' / 'Edit Visible' GameObjects in the current scene. You can remove it, but lock/unlock/visible states will be reset.\n\n" + 
                                    "This object will not be included in the application build (for Unity 5.0+ only). Delete this object if you want to remove the QHierarchy.\n\n" +
                                    "This object can be hidden if you uncheck \"Show QHierarchy GameObject\" in the settings of the QHierarchy"
                                    , MessageType.Info, true);
    	}
    }
}