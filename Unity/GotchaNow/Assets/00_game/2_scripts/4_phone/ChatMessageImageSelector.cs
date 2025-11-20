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

		[Header("Mum")]
		[SerializeField] private Sprite mumImage;
		[SerializeField] private Sprite mumBackgroundImage;

		[Header("Ikigai Tournament")]
		[SerializeField] private Sprite ikigaiTournamentImage;
		[SerializeField] private Sprite ikigaiTournamentBackgroundImage;

		[Header("Bank")]
		[SerializeField] private Sprite bankImage;
		[SerializeField] private Sprite bankBackgroundImage;

		//PUBLIC
		public Sprite GetSenderImage(string senderName)
		{
			return senderName switch
			{
				"KingBob" => kingBobImage,
				"Honey._.Bear" => honeyBearImage,
				"CeilingFanEnthusast" => ceilingFanEnthusastImage,
				"TurtleHerder" => turtleHerderImage,
				"Mum" => mumImage,
				"Ikigai Tournament" => ikigaiTournamentImage,
				"Bank" => bankImage,
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
				"Mum" => mumBackgroundImage,
				"Ikigai Tournament" => ikigaiTournamentBackgroundImage,
				"Bank" => bankBackgroundImage,
				_ => null,
			};
		}
	}
}
