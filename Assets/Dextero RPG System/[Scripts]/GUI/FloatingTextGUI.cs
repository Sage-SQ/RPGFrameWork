using UnityEngine;
 
public class FloatingTextGUI : MonoBehaviour
{
    // The speed - It should be a value bellow 0.08F or it will move too fast
    private float speed = 2.5f;
	public TextMesh damageText;
 
    //Monster takes damage
    public void SetDmgInfo(string damage, Vector3 pos)
    {
        damageText.text      = ToolTipStyle.Red + damage + ToolTipStyle.EndColor;
        Vector3 offset     = 1.2f* Vector3.up;
        transform.position = pos + offset;
		speed = 1.5f;
        GameObject.Destroy(gameObject, 0.9f);
	}
	
	//Player takes damage text
	public void PlayerDamage(string damage, Vector3 pos, float textSize)
    {
        damageText.text      = ToolTipStyle.Brown + damage.ToString() + ToolTipStyle.EndColor;
		int size = (int)(damageText.fontSize * textSize);
		damageText.fontSize = size;
        Vector3 offset     = 1.2f* Vector3.up;
        transform.position = pos + offset;
		speed = 0.9f;
        GameObject.Destroy(gameObject, 0.3f);
    }
	
	//Info text
	public void SetInfo(string info, Vector3 pos)
    {
        damageText.text      = info.ToString();
        Vector3 offset     = (2 * Vector3.up);
		damageText.fontSize = 100;
        transform.position = pos + offset;
		speed = 0.5f;
        GameObject.Destroy(gameObject, 0.8f);
    }
	
	public void AddToText(string textToAdd){
		damageText.text += ToolTipStyle.Break + textToAdd;
	}

    public void Update()
    {
        transform.Translate(0, speed * Time.deltaTime, 0);
    }
}