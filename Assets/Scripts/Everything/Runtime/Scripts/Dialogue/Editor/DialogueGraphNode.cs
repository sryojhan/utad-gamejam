
namespace Dialogue.Editor
{
    public class Node : UnityEditor.Experimental.GraphView.Node
    {
        public string GUID;
        public string MessageText; // Solo para nodos tipo Data
        public bool IsBranch;      // Flag para distinguir visualmente y al guardar
        public bool EntryPoint = false;
    }
}
