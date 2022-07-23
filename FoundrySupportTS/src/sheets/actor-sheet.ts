// import {onManageActiveEffect, prepareActiveEffectCategories} from "../helpers/effects.mjs";
import * as act from "../documents/actor";
import { DEFAULT_TOKEN } from "@league-of-foundry-developers/foundry-vtt-types/src/foundry/common/constants.mjs";
import { ActorData } from "@league-of-foundry-developers/foundry-vtt-types/src/foundry/common/data/data.mjs";
import { assert } from "console";


interface ActorSheetOptions extends ActorSheet.Options{
	// //set to defaults 
	// token?: TokenDocument | null | undefined;
	// viewPermission: 0 | 1 | 2 | 3 = CONST.DOCUMENT_PERMISSION_LEVELS.LIMITED;
	// closeOnSubmit: boolean = true;
	// submitOnChange: boolean = false;
	// submitOnClose: boolean = false;
	// editable: boolean = true;
	// sheetConfig: boolean = false;
	// baseApplication: string | null = null;
	// width: number | null = null;
	// height: number | "auto" | null = null;
	// top: number | null = null;
	// left: number | null = null;
	// scale: number | null = null;
	// popOut: boolean = true;
	// minimizable: boolean = true;
	// resizable: boolean = false;
	// id: string = "";
	// classes: string[] = [];
	// title: string = "";
	// template: string | null = null;
	// scrollY: string[] = [];
	// tabs: Omit<TabsConfiguration, "callback">[] = [];
	// dragDrop: Omit<DragDropConfiguration, "permissions" | "callbacks">[] = [];
	// filters: Omit<SearchFilterConfiguration, "callback">[] = [];

}
interface ActorSheetData extends ActorSheet.Data<ActorSheetOptions>{
	flags: Record<string, unknown>;
	rollData: act.ActorRollData;
	moves: ActorData['items'];
}
/**
 * Extend the basic ActorSheet with some very simple modifications
 * @extends {ActorSheet}
 */
export class PokeroleActorSheet extends ActorSheet{//<ActorSheetOptions,ActorSheetData> {
	/** @override */
	static get defaultOptions() {
		return mergeObject(super.defaultOptions, {
			classes: ["Pokerole", "sheet", "actor"],
			template: "systems/Pokerole/templates/actor/actor-sheet.html",
			width: 600,
			height: 600,
			tabs: [{ navSelector: ".sheet-tabs", contentSelector: ".sheet-body", initial: "primary" }]
		});
	}
	// protected async _render(force?: boolean | undefined, options?: Application.RenderOptions<ActorSheet.Options>
	// 	| undefined): Promise<void> {
	// 	super._render(force, options);
	// 	// this.
	// }
	/** @override */
	get template() {
		return `systems/Pokerole/templates/actor/actor-${this.actor.data.type}-sheet.html`;
	}

	/* -------------------------------------------- */

	/** @override */
	getData() {
		// Retrieve the data structure from the base sheet. You can inspect or log
		// the context variable to see the structure, but some key properties for
		// sheets are the actor object, the data object, whether or not it's
		// editable, the items array, and the effects array.
		const context = super.getData();

		// Use a safe clone of the actor data for further operations.
		const actorData = this.actor.data.toObject(false);

		// Add the actor's data to context.data for easier access, as well as flags.
		var realContext = context as unknown as ActorSheetData;
		realContext.data = actorData.data as any;
		realContext.flags = actorData.flags;

		// Prepare character data and items.
		if (actorData.type == 'pokemon') {
			this._prepareMonItems(realContext);
			this._prepareCharacterData(realContext);
		}

		// Add roll data for TinyMCE editors.
		realContext.rollData = realContext.actor.getRollData() as act.ActorRollData;

		// Prepare active effects
		// realContext.effects = prepareActiveEffectCategories(this.actor.effects);

		return context;
	}

	/**
	 * Organize and classify Items for Character sheets.
	 *
	 * @param {Object} actorData The actor to prepare.
	 *
	 * @return {undefined}
	 */
	_prepareCharacterData(context: ActorSheetData) {
		return new Object();
	}

	/**
	 * Organize and classify Items for Character sheets.
	 *
	 * @param {Object} actorData The actor to prepare.
	 *
	 * @return {undefined}
	 */
	_prepareMonItems(context: ActorSheetData) {
		// Initialize containers.
		const gear = [];
		const features = [];
		const moves = [];
		const types = [];
		const accessories = [];
		// // Iterate through items, allocating to containers
		// for (let i of context.items) {
		//   i.img = i.img || DEFAULT_TOKEN;
		//   // Append to gear.
		//   if (i.type === 'item') {
		//     gear.push(i);
		//   }
		//   // Append to features.
		//   else if (i.type === 'feature') {
		//     features.push(i);
		//   }
		// }

		// // Assign and return
		// context.gear = gear;
		// context.features = features;

		// // Iterate through items, allocating to containers
		// for (let i of context.items) {
		// 	i.img = i.img || DEFAULT_TOKEN;
		// 	if (i.type === 'move') {
		// 		moves.push(i);
		// 	}
		// 	if (i.type === 'type') {
		// 		types.push(i);
		// 	}
		// 	if (i.type === 'accessory') {
		// 		accessories.push(i);
		// 	}
		// 	// Append to gear.
		// 	if (i.type === 'item') {
		// 		gear.push(i);
		// 	}
		// 	// Append to features.
		// 	else if (i.type === 'feature') {
		// 		features.push(i);
		// 	}
		// }
		// // Assign and return
		// context.moves.clear = moves;
		// context.gear = gear;
		// context.features = features;
	}
	protected _injectHTML(html: JQuery<HTMLElement>): void {
		super._injectHTML(html);
		var svgTarget = html.find(".svg-target");
		if (svgTarget === null || svgTarget.length < 1) {
			return;
		}
		var embed = svgTarget.get(0)! as HTMLEmbedElement;
		var svg = embed.getSVGDocument()!;
		if (svg == null) {
			//stupid race condition... run it later
			var thisRef = this;
			embed.addEventListener("load", function () {
				//var t = this;Apparently, 'this' is now embed?
				var svg = this.getSVGDocument();
				if (svg == null) {
					throw new Error("Svg content missing");
				}
				thisRef._markUpSvg(svg);
			});
			return;
		}
		this._markUpSvg(svg);
	}
	private _markUpSvg(svg: Document) {

		//bake the inkscape IDs in. We could do this ahead of time, but that would require writing a tool to
		//do that... Why waste time figuring that out if we can just do it here?
		var items: HTMLElement[] = [];
		$("*", svg).each((index, element) => {
			var inkAttr = element.getAttribute("inkscape:label");
			if (inkAttr === null) {
				return;
			}
			//don't make changes in here... Just in case...
			items.push(element);
		});

		for (var element of items) {
			var inkId = element.getAttribute("inkscape:label")!;
			element.id = inkId;
			//so we can list them all later. I don't know if we can just grab references here and use them later,
			//so I am just setting classes first
			element.classList.add("svg-item");
		}

		// for (const element of svg.getElementsByTagName("*")) {
			
		// }

		// var elements = svg.evaluate("//[inkscape:label]", svg, label => {
		// 	return label === "inkscape" ? "http://www.inkscape.org/namespaces/inkscape" : "";
		// }, XPathResult.ORDERED_NODE_ITERATOR_TYPE);
		// var items = [];
		// for (var item = elements.iterateNext(); item !== null; item = elements.iterateNext()){
		// 	items.push(item as Element);
		// }

		// //apparently node list isn't iterable but has a forEach method?
		// var found = 0;
		// svg.querySelectorAll<SVGElement>("[label]").forEach((element, index, parent) => {
		// 	var realId = element.getAttribute("inkscape:label");
		// 	found++;
		// });
		console.log(`Found ${items.length} svg elements`);		
	}
	/* -------------------------------------------- */
	render(force?: boolean | undefined, options?: Application.RenderOptions<ActorSheet.Options> | undefined): this {
		super.render(force, options);
		//don't really care about the args. We are just here to fill in stat circles and such.
		//and collect things... That too...
		var base = this._element!;here?
		//find me my svg items!
		var svgItems = base.find(".svg-item").toArray();
		console.log(`Found ${svgItems.length} items during render`);
		return this;
	}
	
	/** @override */
	activateListeners(html: JQuery) {
		super.activateListeners(html);

		// Render the item sheet for viewing/editing prior to the editable check.
		html.find('.item-edit').click(ev => {
			const li = $(ev.currentTarget).parents(".item");
			const item = this.actor.items.get(li.data("itemId"));
			item?.sheet?.render(true);
		});

		// -------------------------------------------------------------
		// Everything below here is only needed if the sheet is editable
		if (!this.isEditable) return;

		


		// Add Inventory Item
		html.find('.item-create').click(this._onItemCreate.bind(this));

		// Delete Inventory Item
		html.find('.item-delete').click(ev => {
			const li = $(ev.currentTarget).parents(".item");
			const item = this.actor.items.get(li.data("itemId"));
			item!.delete();
			li.slideUp(200, () => this.render(false));
		});

		// Active Effect management
		// html.find(".effect-control").click(ev => onManageActiveEffect(ev, this.actor));

		// Rollable abilities.
		html.find('.rollable').click(this._onRoll.bind(this));

		// Drag events for macros.
		if ((this.actor as any).owner) {//figure out missing owner field later...
			let handler = (ev: DragEvent) => this._onDragStart(ev);
			html.find('li.item').each((i, li) => {
				if (li.classList.contains("inventory-header")) return;
				li.setAttribute("draggable", true + "");
				li.addEventListener("dragstart", handler, false);
			});
		}

	}

	/**
	 * Handle creating a new Owned Item for the actor using initial data defined in the HTML dataset
	 * @param {Event} event   The originating click event
	 * @private
	 */
	async _onItemCreate(event: Event) {
		event.preventDefault();
		const header = event.currentTarget as any;
		// Get the type of item to create.
		const type = header.dataset.type;
		// Grab any data associated with this control.
		const data = duplicate(header.dataset);
		// Initialize a default name.
		const name = `New $ {type.capitalize()}`;
		// Prepare the item object.
		const itemData = {
			name: name,
			type: type,
			data: data
		};
		// Remove the type from the dataset since it's in the itemData.type prop.
		delete itemData.data["type"];

		// Finally, create the item!
		return await Item.create(itemData, {parent: this.actor});
	}

	/**
	 * Handle clickable rolls.
	 * @param {Event} event   The originating click event
	 * @private
	 */
	_onRoll(event: Event) {
		event.preventDefault();
		const element = event.currentTarget as any;
		const dataset = element.dataset;

		// Handle item rolls.
		if (dataset.rollType) {
			if (dataset.rollType == 'item') {
				const itemId = element.closest('.item').dataset.itemId;
				const item = this.actor.items.get(itemId);
				if (item) return (item as any).roll();//resolve missing method later
			}
		}

		// Handle rolls that supply the formula directly.
		if (dataset.roll) {
			let label = dataset.label ? `[roll] $ {dataset.label}` : '';
			let roll = new Roll(dataset.roll, this.actor.getRollData());
			roll.toMessage({
				speaker: ChatMessage.getSpeaker({ actor: this.actor }),
				flavor: label,
				rollMode: (game as Game).settings.get('core', 'rollMode'),
			});
			return roll;
		}
	}

}
