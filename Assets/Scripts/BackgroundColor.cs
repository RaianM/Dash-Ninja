using UnityEngine;

[CreateAssetMenu(menuName = "Background Color")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player
public class BackgroundColor : ScriptableObject
{
	[Header("Color")]
	public int red;
	public int green;
	public int blue;
}