using UnityEngine;

namespace GotchaNow
{
	public class ChatMessageImageSelector : MonoBehaviour
	{
		[Header("Sender Images")]
		[Header("KingBob")]
		[SerializeField] private Sprite kingBobImage;
		[SerializeField] private Sprite kingBobBackgroundImage;

		[Header("Honey._.Bear")]
		[SerializeField] private Sprite honeyBearImage;
		[SerializeField] private Sprite honeyBearBackgroundImage;

		[Header("CeilingFanEnthusast")]
		[SerializeField] private Sprite ceilingFanEnthusastImage;
		[SerializeField] private Sprite ceilingFanEnthusastBackgroundImage;

		[Header("TurtleHerder")]
		[SerializeField] private Sprite turtleHerderImage;
		[SerializeField] private Sprite turtleHerderBackgroundImage;
		[Header("Ikigai Tournament")]
		[SerializeField] private Sprite ikigaiTournamentImage;
		[SerializeField] private Sprite ikigaiTournamentBackgroundImage;

		//PUBLIC
		public Sprite GetSenderImage(string senderName)
		{
			return senderName switch
			{
				"KingBob" => kingBobImage,
				"Honey._.Bear" => honeyBearImage,
				"CeilingFanEnthusast" => ceilingFanEnthusastImage,
				"TurtleHerder" => turtleHerderImage,
				"Ikigai Tournament" => ikigaiTournamentImage,
				_ => null,
			};
		}

		public Sprite GetSenderBackgroundImage(string senderName)
		{
			return senderName switch
			{
				"KingBob" => kingBobBackgroundImage,
				"Honey._.Bear" => honeyBearBackgroundImage,
				"CeilingFanEnthusast" => ceilingFanEnthusastBackgroundImage,
				"TurtleHerder" => turtleHerderBackgroundImage,
				"Ikigai Tournament" => ikigaiTournamentBackgroundImage,
				_ => null,
			};
		}
	}
}
