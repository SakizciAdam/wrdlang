using System;
using System.Collections.Generic;
using WrdLang.Lang.Lex;
using System.Dynamic;

namespace WrdLang.Lang.VM.Types {
    public class LClass : DynamicObject
{
    public Dictionary<string, KeyValuePair<Type, object>> _fields;

    public LClass(List<Field> fields)
    {
        _fields = new Dictionary<string, KeyValuePair<Type, object>>();
        fields.ForEach(x => _fields.Add(x.FieldName,
            new KeyValuePair<Type, object>(x.FieldType, null)));
    }

    public LClass()
    {
        _fields = new Dictionary<string, KeyValuePair<Type, object>>();

        
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        if (_fields.ContainsKey(binder.Name))
        {
            var type = _fields[binder.Name].Key;
            if (value.GetType() == type)
            {
                _fields[binder.Name] = new KeyValuePair<Type, object>(type, value);
                return true;
            }
            else throw new Exception("Value " + value + " is not of type " + type.Name);
        }
        return false;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        result = _fields[binder.Name].Value;
        return true;
    }
}
public class Field
{
    public Field(string name, Type type)
    {
        this.FieldName = name;
        this.FieldType = type;
    }

    public string FieldName;

    public Type FieldType;
}

}