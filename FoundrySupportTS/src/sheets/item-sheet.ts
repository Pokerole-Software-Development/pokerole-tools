
interface ItemSheetOptions extends ItemSheet.Options{

}
interface ItemSheetData extends ItemSheet.Data<ItemSheetOptions>{

}

/**
 * Extend the basic ItemSheet with some very simple modifications
 * @extends {ItemSheet}
 */
export class PokeroleItemSheet extends ItemSheet<ItemSheetOptions, ItemSheetData> {

	/** @override */
	static get defaultOptions() {
		return mergeObject(super.defaultOptions, {
			classes: ["Pokerole", "sheet", "item"],
			width: 520,
			height: 480,
			tabs: [{ navSelector: ".sheet-tabs", contentSelector: ".sheet-body", initial: "description" }]
		});
	}

	/** @override */
	get template() {
		const path = "systems/Pokerole/templates/item";
		// Return a single sheet for all item types.
		// return \`$ {path}/item-sheet.html\`;

		// Alternatively, you could use the following return statement to do a
		// unique item sheet by type, like \`weapon-sheet.html\`.
		return `${path}/item-${this.item.data.type}-sheet.html`;
	}

	/* -------------------------------------------- */

	/** @override */
	getData(options?: Partial<ItemSheetOptions>) {
		// Retrieve base data structure.
		const context = super.getData(options) as ItemSheetData;

		// Use a safe clone of the item data for further operations.
		const itemData = context.item.data;//.toObject(false);

		// Retrieve the roll data for TinyMCE editors.
		// context.rollData = {};
		// let actor = this.object?.parent ?? null;
		// if (actor) {
		// 	context.rollData = actor.getRollData();
		// }

		// // Add the actor's data to context.data for easier access, as well as flags.
		// context.data = itemData.data;
		// context.flags = itemData.flags;

		return context;
	}

	/* -------------------------------------------- */

	/** @override */
	activateListeners(html: JQuery<HTMLElement>) {
		super.activateListeners(html);

		// Everything below here is only needed if the sheet is editable
		if (!this.isEditable) return;

		// Roll handlers, click handlers, etc. would go here.
	}
}
