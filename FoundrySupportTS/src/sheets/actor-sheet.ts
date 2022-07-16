// import {onManageActiveEffect, prepareActiveEffectCategories} from "../helpers/effects.mjs";
import { ActorRollData } from "../documents/actor";
import { DEFAULT_TOKEN } from "@league-of-foundry-developers/foundry-vtt-types/src/foundry/common/constants.mjs";
import { ActorData } from "@league-of-foundry-developers/foundry-vtt-types/src/foundry/common/data/data.mjs";

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
	rollData: ActorRollData;
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
	protected async _render(force?: boolean | undefined, options?: Application.RenderOptions<ActorSheet.Options>
		| undefined): Promise<void> {
		super._render(force, options);
		// this.
	}
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
		if (actorData.type == 'character') {
			this._prepareMonItems(realContext);
			this._prepareCharacterData(realContext);
		}

		// Add roll data for TinyMCE editors.
		realContext.rollData = realContext.actor.getRollData() as ActorRollData;

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

		// Iterate through items, allocating to containers
		for (let i of context.items) {
			i.img = i.img || DEFAULT_TOKEN;
			if (i.type === 'move') {
				moves.push(i);
			}
			if (i.type === 'type') {
				types.push(i);
			}
			if (i.type === 'accessory') {
				accessories.push(i);
			}
			// Append to gear.
			if (i.type === 'item') {
				gear.push(i);
			}
			// Append to features.
			else if (i.type === 'feature') {
				features.push(i);
			}
		}
		// Assign and return
		context.moves.clear = moves;
		context.gear = gear;
		context.features = features;
	}

	/* -------------------------------------------- */

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
