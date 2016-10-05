﻿using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR.Handles;
using UnityEngine.VR.Modules;
using UnityEngine.VR.UI;
using UnityEngine.VR.Utilities;
using Object = UnityEngine.Object;

public class InspectorObjectFieldItem : InspectorPropertyItem
{
	[SerializeField]
	Text m_FieldLabel;

	Type m_ObjectType;
	string m_ObjectTypeName;

	public override void Setup(InspectorData data)
	{
		base.Setup(data);

		m_ObjectTypeName = U.Object.NiceSerializedPropertyType(m_SerializedProperty.type);
		m_ObjectType = U.Object.TypeNameToType(m_ObjectTypeName);

		SetObject(m_SerializedProperty.objectReferenceValue);
	}

	bool SetObject(Object obj)
	{
		var objectReference = m_SerializedProperty.objectReferenceValue;

		if (obj == null)
			m_FieldLabel.text = string.Format("None ({0})", m_ObjectTypeName);
		else
		{
			var objType = obj.GetType();
			if (!objType.IsAssignableFrom(m_ObjectType))
			{
				if (obj.Equals(objectReference)) // Show type mismatch for old serialized data
					m_FieldLabel.text = "Type Mismatch";
				return false;
			}
			m_FieldLabel.text = string.Format("{0} ({1})", obj.name, obj.GetType().Name);
		}

		if (obj == null && m_SerializedProperty.objectReferenceValue == null)
			return true;
		if (m_SerializedProperty.objectReferenceValue != null && m_SerializedProperty.objectReferenceValue.Equals(obj))
			return true;

		m_SerializedProperty.objectReferenceValue = obj;

		data.serializedObject.ApplyModifiedProperties();

		return true;
	}

	protected override void OnDragEnded(BaseHandle baseHandle, HandleEventData eventData)
	{
		var fieldBlock = baseHandle.transform.parent;
		object droppedObject = null;
		GameObject target;
		IDropReceiver dropReceiver = getCurrentDropReceiver(eventData.rayOrigin, out target);
		if (dropReceiver != null && fieldBlock)
		{
			var inputfields = fieldBlock.GetComponentsInChildren<NumericInputField>();
			if (inputfields.Length > 1)
			{
				switch (m_SerializedProperty.propertyType)
				{
					case SerializedPropertyType.Vector2:
						droppedObject = m_SerializedProperty.vector2Value;
						break;
					case SerializedPropertyType.Quaternion:
						droppedObject = m_SerializedProperty.quaternionValue;
						break;
					case SerializedPropertyType.Vector3:
						droppedObject = m_SerializedProperty.vector3Value;
						break;
					case SerializedPropertyType.Vector4:
						droppedObject = m_SerializedProperty.vector4Value;
						break;
				}
			}
			else if (inputfields.Length > 0)
				droppedObject = inputfields[0].text;

			dropReceiver.ReceiveDrop(target, droppedObject);
		}
		base.OnDragEnded(baseHandle, eventData);
	}

	public void ClearButton()
	{
		SetObject(null);
	}

	protected override object GetDropObject(Transform fieldBlock)
	{
		return m_SerializedProperty.objectReferenceValue;
	}

	public override bool TestDrop(GameObject target, object droppedObject)
	{
		return droppedObject is Object;
	}

	public override bool ReceiveDrop(GameObject target, object droppedObject)
	{
		var obj = droppedObject as Object;
		return obj && SetObject(obj);
	}
}