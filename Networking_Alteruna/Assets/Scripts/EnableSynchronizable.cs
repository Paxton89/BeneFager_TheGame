using UnityEngine;
using Alteruna.Trinity;

public class EnableSynchronizable : Synchronizable
{
	// Data
    private Vector3 mOldPosition;
    private Quaternion mOldRotation;
    private Vector3 mOldScale;

    public void UpdateTransform()
    {
	    Commit();
    }

    private void Start()
    {
        mOldPosition = transform.localPosition;
        mOldRotation = transform.localRotation;
        mOldScale = transform.localScale;
    }
	
	public override void AssembleData(Writer writer)
	{
		writer.Write(gameObject.activeSelf);
		
		mOldPosition = transform.localPosition;
		mOldRotation = transform.localRotation;
		mOldScale = transform.localScale;

		// Position
		writer.Write(transform.localPosition.x);
		writer.Write(transform.localPosition.y);
		writer.Write(transform.localPosition.z);

		// Rotation
		writer.Write(transform.localRotation.eulerAngles.x);
		writer.Write(transform.localRotation.eulerAngles.y);
		writer.Write(transform.localRotation.eulerAngles.z);

		// Scale
		writer.Write(transform.localScale.x);
		writer.Write(transform.localScale.y);
		writer.Write(transform.localScale.z);
	}

	public override void DisassembleData(Reader reader)
	{
		gameObject.SetActive(reader.ReadBool());
		
		transform.localPosition =
			new Vector3(
				reader.ReadFloat(),
				reader.ReadFloat(),
				reader.ReadFloat()
			);

		transform.localRotation =
			Quaternion.Euler(
				reader.ReadFloat(),
				reader.ReadFloat(),
				reader.ReadFloat()
			);

		transform.localScale =
			new Vector3(
				reader.ReadFloat(),
				reader.ReadFloat(),
				reader.ReadFloat()
			);

		mOldPosition = transform.localPosition;
		mOldRotation = transform.localRotation;
		mOldScale = transform.localScale;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		
		Commit();
	}

	private void OnDisable()
	{
		Commit();
	}
}