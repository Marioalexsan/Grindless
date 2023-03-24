using System.Reflection;
using static SoG.SpellVariable;

namespace Grindless
{
    public class SpellVariableEditor
    {
        private static FieldInfo _denfVariablesField = AccessTools.Field(typeof(SpellVariable), "denfVariables");
        private static Dictionary<Handle, float> DenfVariables => (Dictionary<Handle, float>)_denfVariablesField.GetValue(null);

        private readonly Dictionary<Handle, float> _previousValues = new();

        public void EditValue(Handle handle, float value)
        {
            if (!_previousValues.ContainsKey(handle))
            {
                _previousValues[handle] = DenfVariables[handle];
            }

            DenfVariables[handle] = value;
        }

        public void EditValueBy(Handle handle, float change)
        {
            EditValue(handle, Get(handle) + change);
        }

        public void RestoreValues()
        {
            foreach (var pair in _previousValues)
            {
                DenfVariables[pair.Key] = pair.Value;
            }

            _previousValues.Clear();
        }
    }
}
