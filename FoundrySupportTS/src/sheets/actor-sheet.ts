// import {onManageActiveEffect, prepareActiveEffectCategories} from "../helpers/effects.mjs";
import * as act from "../documents/actor.js";
import { DEFAULT_TOKEN } from "@league-of-foundry-developers/foundry-vtt-types/src/foundry/common/constants.mjs";
import { ActorData } from "@league-of-foundry-developers/foundry-vtt-types/src/foundry/common/data/data.mjs";
import { assert } from "console";
import { POKEROLE } from "../helpers/config.js";
import { stringify } from "querystring";


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
	// data: act.PokeroleActorData;
}
interface ActorSheetData extends ActorSheet.Data<ActorSheetOptions>{
	flags: Record<string, unknown>;
	rollData: act.ActorRollData;
	moves: ActorData['items'];
	dotData: Map<string, DotInfo>;
}
interface DotInfo {
	min: number;
	val: number;
	max: number;
	/**
	 * Dots for this stat. The item for i is at i - 1
	 */
	dots: HTMLElement[];
	// group: HTMLElement;
}
/**
 * Extend the basic ActorSheet with some very simple modifications
 * @extends {ActorSheet}
 */
export class PokeroleActorSheet extends ActorSheet<ActorSheetOptions,ActorSheetData> {
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
	//Note: \w includes \d
	private static readonly dotFinder: RegExp = /^(\w+)\-(\d+)$/;
	private static dotParentFinder: RegExp = /^(\w+)\ Dots$/;
	/** @override */
	get template() {
		return `systems/Pokerole/templates/actor/actor-${this.actor.data.type}-sheet.html`;
	}

	/**
	 * Pristine copy of the sheet's svg element. This is stored after baking IDs
	 */
	private _cachedPristineSvg?: HTMLElement;
	/* -------------------------------------------- */
	/**
	 * Cached result of {@link getData} for svg operations since the svg won't be ready right away
	 */
	private _svgContextCache?: ActorSheetData = undefined;
	// private _svgCached?: HTMLElement;
	/** @override */
	async getData(options?: Partial<ActorSheetOptions>) {
		//base impl is awaited anyway
		// Retrieve the data structure from the base sheet. You can inspect or log
		// the context variable to see the structure, but some key properties for
		// sheets are the actor object, the data object, whether or not it's
		// editable, the items array, and the effects array.
		const context = (await super.getData(options)) as ActorSheetData;

		// Use a safe clone of the actor data for further operations.
		const actorData = this.actor.data.toObject(false);

		// Add the actor's data to context.data for easier access, as well as flags.
		// var realContext = context as unknown as ActorSheetData;
		context.data = actorData.data as any;//figure that type issue out later
		context.flags = actorData.flags;

		// Prepare character data and items.
		if (actorData.type == POKEROLE.ActorTypes.mon) {
			this._prepareMonItems(context);
			this._prepareCharacterData(context);
		}

		// Add roll data for TinyMCE editors.
		context.rollData = context.actor.getRollData() as act.ActorRollData;

		// Prepare active effects
		// realContext.effects = prepareActiveEffectCategories(this.actor.effects);
		this._svgContextCache = context;
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
	private _cacheSvg(svg: Document) {
		//this is to avoid needing to load the svg over when the form goes for an update. Because loading
		//the svg again results in flicker
		if (this._cachedPristineSvg) {
			return;
		}
		this._cachedPristineSvg = this._markUpSvg(svg);
	}
	private _findAndInitSvg(html: JQuery) {
		var svgTarget = html.find(".svg-target");
		if (svgTarget === null || svgTarget.length < 1) {
			return;
		}
		var embed = svgTarget.get(0)! as HTMLEmbedElement;
		//do we have a cached copy?
		if (!this._cachedPristineSvg) {
			//we will need to get it later...
			var thisRef = this;
			const initFunc = function () {
				var svg = embed.getSVGDocument();
				if (svg == null) {
					throw new Error("Svg content missing");
				}
				thisRef._cacheSvg(svg);
				//so it doesn't get run more than once
				embed.removeEventListener("load", initFunc);
				var copy = thisRef._cachedPristineSvg!.cloneNode(true) as HTMLElement;
				thisRef._initSvg(copy, embed);
			};
			//stupid race condition... run it later
			embed.addEventListener("load", initFunc);
			return;
		}
		var copy = this._cachedPristineSvg.cloneNode(true) as HTMLElement;
		this._initSvg(copy, embed);
	}
	private _initSvg(svg: HTMLElement, oldParent: HTMLEmbedElement) {
		//find my items...
		var items = svg.getElementsByClassName("svg-item");
		//now we need to figure out what is what
		this._assignSvgObjects(Array.from(items) as HTMLElement[]);
		oldParent.replaceWith(svg);
	}
	private _markUpSvg(svg: Document) : HTMLElement {
		//bake the inkscape IDs in. We could do this ahead of time, but that would require writing a tool to
		//do that... Why waste time figuring that out if we can just do it here?
		var items: HTMLElement[] = [];
		$("*", svg).each((_, element) => {
			var inkAttr = element.getAttribute("inkscape:label");
			if (inkAttr === null) {
				return;
			}
			//These ones DON'T COUNT!
			switch (inkAttr) {
				case "Layer 1":
				case "POKEMON SHEET 2.0":
					return;
				default:
					if (/(?:path|g)\d+/.test(inkAttr)) {
						//no
						return;
					}
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
		console.log(`Found ${items.length} svg elements`);
		var root = svg.documentElement;
		root.remove();
		return root;
	}
	_assignSvgObjects(svgItems: HTMLElement[]) {
		const context = this._svgContextCache;
		if (context === undefined) {
			throw new Error("Missing data context for SVG");
		}
		//Debug: Did we mark everything?
		// if (true) {
		// 	for (var item of svgItems) {
		// 		item.style.fill = "cyan";
		// 		// item.style.border = "solid";
		// 		// item.style.borderColor = "#00FFFF";
		// 		// item.style.borderWidth = "5px";
		// 		// item.style.border
		// 	}need to mark the move boxes. Will do that later
		// }
		const isMon = context.actor.type === POKEROLE.ActorTypes.mon;
		var textAreas = [];
		var clickAreas = [];
		for (var item of svgItems) {
			var matchResult = PokeroleActorSheet.dotFinder.exec(item.id);
			if (matchResult !== null) {
				var stat = matchResult[1]!;
				var dotNum = parseInt(matchResult[2]!)
				if (!context.dotData) {
					//init that...
					context.dotData = new Map();
				}
				var dotInfo = context.dotData.get(stat);
				if (dotInfo === undefined) {
					//init that
					var minMaxData = this._getMinMaxDotData(isMon, stat);
					//find group parent
					
					dotInfo = {
						min: minMaxData.min,
						max: minMaxData.max,
						dots: [],
						val: minMaxData.min,//we don't know yet...
					};
					//add to the map
					context.dotData.set(stat, dotInfo);
				}
				if (dotNum > dotInfo.max) {
					throw new Error(`stat dot for ${stat} (${dotNum}) was greater than the max of ${dotInfo.max}`);
				}
				dotInfo.dots[dotNum - 1] = item;
				continue;
			}
			if (item.id.startsWith("click_")) {
				clickAreas.push(item);
				continue;
			}
			//text area
			textAreas.push(item);
		}
		//fill in dem dots
		for (var pair of context.dotData) {
			const statName = pair[0];
			const dotInfo = pair[1];
			this._setStatSvg(context, pair[0], pair[1]);
			//add click listeners to them if not readonly
			if (this.isEditable) {
				// make a "text input field" so the stupid data will persist
				var parentGroup = dotInfo.dots[0]!;
				const dots = dotInfo.dots;
				var thisRef = this;
				//add click listeners to them
				for (var i = 0; i < dots.length; i++){
					var dot = dots[i]!;
					const iRef = i;
					dot.addEventListener("click", (ev) => {
						ev.preventDefault();
						var target = ev.button == 2 ? iRef : iRef + 1;
						var dataObj = <act.PlayerActorData>this.actor.data.data;
						act.setStat(dataObj, statName, target);
						this._setStatSvg0(dataObj, statName, dotInfo);
						//send update information
						// var dataPath = `data.${statName}`;
						var dataPartial = {} as any;
						dataPartial[statName] = target;
						this.actor.update({
							data: dataPartial
						}, );
						// thisRef._setStatSvg()
					});
				}
			}
		}
		//handle named areas
		//remember to put text elements OVER the SVG so they will refresh properly
		for (var item of textAreas) {
			switch (item.id) {
				case 'nature':
					// var select = new HTMLSelectElement();
					// var options = select.options;
					// options.
					// select.
					// item.
			}
		}

	}
	private _getMinMaxDotData(isMon: boolean, stat: string): { min: number, max: number } {
		switch (stat) {
			case 'strength':
			case 'dexterity':
			case 'vitality':
			case 'special':
			case 'insight':
				var max = isMon ? 12 : 5;
				return { min: 1, max: max };
			default:
				return { min: 0, max: 5 };
		}
	}
	private _setStatSvg(context: ActorSheetData, stat: string, dotInfo: DotInfo) {
		if (context.data.type === POKEROLE.ActorTypes.rival) {
			return;
		}
		this._setStatSvg0(context.data as unknown as act.PlayerActorData, stat, dotInfo);
	}
	private _setStatSvg0(data: act.PlayerActorData, stat: string, dotInfo: DotInfo) {
		// just double checking, these should all be paths
		//supporting circles since I want to change them to that eventually...
		for (var dot of dotInfo.dots) {
			if (dot.tagName !== "path" && dot.tagName !== "circle") {
				//svg set up wrong
				throw new Error(`Named dot (${dot.id}) was not an svg path!`)
			}
		}
		var num = act.getStat(data, stat);
		if (num === undefined) {
			//apparently not set yet...
			num = 0;
		}
		if (num < dotInfo.min) {
			num = dotInfo.min;
		}
		for (var i = 0; i < dotInfo.dots.length; i++){
			var on = num > i;//well, techinically num>=i+1, but that takes more text
			var dot = dotInfo.dots[i]!;
			//black if on, white if not
			var color = on ? "#000000" : "#ffffff";
			dot.style.fill = color;
		}
	}
	/** @override */
	activateListeners(html: JQuery) {
		super.activateListeners(html);
		//don't really have anywhere else to do this... so...
		this._findAndInitSvg(html);

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
	_svgAddListeners(svg: Document) {
		
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

