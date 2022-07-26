These SVG files have a number of elements that can be addressed by name and modified.
All stats dots follow the pattern of `lowercase(statname)-(dot number)`. So, like `strength-1` for the first dot of Strength and `misc1-1` is the first dot of the first misc skill
The Rival sheets has a number of "blocker" elements over some items. If you wish to have, say, "Hax0rus" visible, set the visibility of the object/group of that name to false and it will be visible.
Checkmarks are not supported at this time
There are additional named SVG items to allow for the poisioning of text. They may be on paths, groups, or invisible rectangles, so keep an eye out for those. They are listed below:
* Common:
	* name: Name of the character
	* hp: The full box for HP.
	* will: The full box for Will.
* Pokémon:
	* dexnum: The pokedex number for the pokémon
	* ability: Pokémon's abilty
	* picture: Picture of the character. It may be clipped, in the case of the pokemon
	* item: The pokémon's held item
	* status: Status of the 'mon
	* initiative: 'mons calculated initiative
	* accuracy: "Most commonly used accuracy dice pool"... Not really applicable with a VTT, so we will put the last used accuracy pool here
	* damage: "Most commonly used damage dice pool"... Not really applicable with a VTT, so we will put the last used damage pool here
	* evasion: Calculated dice pool for evading
	* clash: Dice pool used for clashing

The following elements are named so you can add click handlers on them:
* Common:
	* click_will: Positioned over the WILL text for the Will text element
* Pokémon
	* click_initiative: Positioned over the INITIATIVE text
	* click_evasion: Rolls evasion
	* click_clash: Rolls Clash
