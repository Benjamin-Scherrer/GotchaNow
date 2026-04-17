using UnityEngine;
using UnityEngine.UI;

namespace GotchaNow
{
	public class SelectableLink : Selectable
	{
		[SerializeField] private Selectable selectableLink;
        public Selectable GetSelectableLink { get { return selectableLink; } }
    }
}
