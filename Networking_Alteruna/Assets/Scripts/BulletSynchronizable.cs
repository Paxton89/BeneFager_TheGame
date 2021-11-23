using UnityEngine;
using Alteruna.Trinity;
using Unity.VisualScripting;

public class BulletSynchronizable : Synchronizable
{
	// Data
    private Vector3 mOldPosition;
    private Quaternion mOldRotation;
    private Vector3 mOldScale;
    private bool mOldActiveState;
    private Transform _tf;

    public override void DisassembleData(Reader reader)
    {
        _tf.gameObject.SetActive(reader.ReadBool());
        
        _tf.localPosition =
            new Vector3(
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat()
            );

        _tf.localRotation =
            Quaternion.Euler(
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat()
            );

        _tf.localScale =
            new Vector3(
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat()
            );

        mOldPosition = _tf.localPosition;
        mOldRotation = _tf.localRotation;
        mOldScale = _tf.localScale;
        mOldActiveState = _tf.gameObject.activeSelf;
    }

    public override void AssembleData(Writer writer)
    {
        mOldPosition = _tf.localPosition;
        mOldRotation = _tf.localRotation;
        mOldScale = _tf.localScale;
        mOldActiveState = _tf.gameObject.activeSelf;

        writer.Write(_tf.gameObject.activeSelf);
        
        // Position
        writer.Write(_tf.localPosition.x);
        writer.Write(_tf.localPosition.y);
        writer.Write(_tf.localPosition.z);

        // Rotation
        writer.Write(_tf.localRotation.eulerAngles.x);
        writer.Write(_tf.localRotation.eulerAngles.y);
        writer.Write(_tf.localRotation.eulerAngles.z);

        // Scale
        writer.Write(_tf.localScale.x);
        writer.Write(_tf.localScale.y);
        writer.Write(_tf.localScale.z);
    }

    private void Update()
    {
        if (mOldPosition != _tf.localPosition ||
            mOldRotation != _tf.localRotation ||
            mOldScale != _tf.localScale ||
            mOldActiveState != _tf.gameObject.activeSelf)
        {
            Commit();
        }
    }

    private void Awake()
    {
        Debug.Log(gameObject.name);
        _tf = transform.GetChild(0);
    }

    private void Start()
    {
        mOldPosition = _tf.localPosition;
        mOldRotation = _tf.localRotation;
        mOldScale = _tf.localScale;
        mOldActiveState = _tf.gameObject.activeSelf;
    }
}