import { MoveItemData, PokeroleItemData, PokeroleItem } from "../documents/item.js";
import { POKEROLE } from "../helpers/config.js";

interface ItemSheetOptions extends ItemSheet.Options{

}
interface ItemSheetData extends ItemSheet.Data<ItemSheetOptions>{
	// readonly system: Object;
	readonly item: PokeroleItem & this["document"];
	readonly system: PokeroleItemData;
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
			tabs: [{ navSelector: ".sheet-tabs", contentSelector: ".sheet-body", initial: "primary" }]
		});
	}
	//cache that
	private static monStats: readonly string[] = POKEROLE.StatsUtils.getStatList("mon");
	/** @override */
	get template() {
		const path = "systems/Pokerole/templates/item";
		// Return a single sheet for all item types.
		// return \`$ {path}/item-sheet.html\`;

		// Alternatively, you could use the following return statement to do a
		// unique item sheet by type, like \`weapon-sheet.html\`.
		return `${path}/item-${this.item.type}-sheet.html`;
	}
	private _cachedContext?: PokeroleItemData;

	/* -------------------------------------------- */

	/** @override */
	getData(options?: Partial<ItemSheetOptions>) {
		// Retrieve base data structure.
		const context = super.getData(options) as ItemSheetData;
		// Use a safe clone of the item data for further operations.
		const itemCopy = context.item.toObject(false);
		//const itemData = itemCopy.system.toObject(false);// system;//.toObject(false);
		//shush you
		const item = (itemCopy as any).system as PokeroleItemData;
		this._cachedContext = item;
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

	protected async _renderInner(data: ItemSheetData): Promise<JQuery<HTMLElement>> {
		var result = await super._renderInner(data);
		if (!this._cachedContext//just in case...
			|| this.item.type !== 'move') {
			return result;
		}
		var moveData = data.item.system as unknown as MoveItemData;
		// var rootDoc = result.do it is too late to add the items in...
		// //fill in combo box choices
		// var options = [];
		// for (var stat of PokeroleItemSheet.monStats) {
		// 	var el = new HTMLOptionElement();
		// 	el.value = stat;
		// 	el.text = stat;
		// 	options.push(el);
		// }
		//find insertion points


		this.applyTypeColor(moveData.type, result);
		return result;
		// if (this.item.type === 'move') {
		// 	this.applyTypeColor((this._cachedContext as MoveItemData).type, html);
		// }
		// super._injectHTML(html);
	}

	/** @override */
	activateListeners(html: JQuery<HTMLElement>) {
		super.activateListeners(html);

		// Everything below here is only needed if the sheet is editable
		if (!this.isEditable) return;

		if (this.item.type === 'move') {
			html.find(".type.combobox").on('change', e => {
				var combobox = e.target! as HTMLSelectElement;
				this.applyTypeColor(combobox);
			});
		}
		// Roll handlers, click handlers, etc. would go here.
	}
	applyTypeColor(typename: string | HTMLSelectElement, root?: JQuery<HTMLElement>) {
		if (typename instanceof HTMLSelectElement) {
			//get the type from that
			typename = typename.value;
		}
		if (!root) {
			root = this.element;
		}
		var color = POKEROLE.TypeManager.getColorForType(typename);
		if (color === null || color === undefined) {
			// use the default colors
			root.css("background-color", "unset");
			root.css("color", "unset");
			return;
		}
		var readableColor = POKEROLE.getReadableColor(color);
		var element = root[0]!;
		var style = element.style;
		//truncate background color since apparently, alpha is last???
		//setting alpha to 255 in case it isn't
		 color = color | 0xff000000; //breaks things???
		style.backgroundColor = `#${color.toString(16)};`;
		style.color = `#${readableColor.toString(16)};`;
	}
}
