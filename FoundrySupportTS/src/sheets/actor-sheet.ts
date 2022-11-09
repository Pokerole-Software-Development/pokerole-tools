// import {onManageActiveEffect, prepareActiveEffectCategories} from "../helpers/effects.mjs";
// import * as act from "../documents/actor.js";
import { DEFAULT_TOKEN } from "@league-of-foundry-developers/foundry-vtt-types/src/foundry/common/constants.mjs";
import { ActorData } from "@league-of-foundry-developers/foundry-vtt-types/src/foundry/common/data/data.mjs";
// import { assert } from "console";
import { POKEROLE } from "../helpers/config.js";
import { stringify } from "querystring";
import { ActorRollData, getActorStat, PlayerActorData, PokemonActorData, PokeroleActor, PokeroleActorData, setActorStat } from "../documents/actor.js";


interface PokeroleActorSheetOptions extends ActorSheet.Options{
}
//Note: This is apparently what gets passed to handlebars for the html generation
interface PokeroleActorSheetData extends ActorSheet.Data<PokeroleActorSheetOptions>{
	// flags: Record<string, unknown>;
	rollData: ActorRollData;
	moves: ActorData['items'];
	dotData: Map<string, DotInfo>;
	actor: PokeroleActor & this["document"];
	system: PokeroleActorData//PokeroleActor["system"];
}
interface DotInfo {
	min: number;
	val: number;
	max: number;
	/**
	 * Dots for this stat. The item for i is at i - 1
	 */
	dots: SVGElement[];
	// group: HTMLElement;
}
/**
 * Extend the basic ActorSheet with some very simple modifications
 * @extends {ActorSheet}
 */
export class PokeroleActorSheet extends ActorSheet<PokeroleActorSheetOptions,PokeroleActorSheetData> {
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
		return `systems/Pokerole/templates/actor/actor-${this.actor.type}-sheet.html`;
	}

	/**
	 * Pristine copy of the sheet's svg element. This is stored after baking IDs
	 */
	private _cachedPristineSvg?: HTMLElement;
	/* -------------------------------------------- */
	/**
	 * Cached result of {@link getData} for svg operations since the svg won't be ready right away
	 */
	private _svgContextCache?: PokeroleActorSheetData = undefined;
	// private _svgCached?: HTMLElement;
	/** @override */
	async getData(options?: Partial<PokeroleActorSheetOptions>) {
		//base impl is awaited anyway
		// Retrieve the data structure from the base sheet. You can inspect or log
		// the context variable to see the structure, but some key properties for
		// sheets are the actor object, the data object, whether or not it's
		// editable, the items array, and the effects array.
		const context = (await super.getData(options)) as PokeroleActorSheetData;

		// Use a safe clone of the actor data for further operations.
		const actorCopy = context.actor.toObject(false);

		// Add the actor's data to context.data for easier access, as well as flags.
		//stupid dumb, 'system' not defined yet...
		context.system = (actorCopy as any).system as PokeroleActorData;
		context.data = actorCopy;
		// context.flags = actorData.flags;//resolving data -> system change

		// Prepare character data and items.
		if (context.actor.type == POKEROLE.ActorTypes.mon) {
			this._prepareMonItems(context);
			this._prepareCharacterData(context);
		}

		// Add roll data for TinyMCE editors.
		context.rollData = context.actor.getRollData() as ActorRollData;

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
	_prepareCharacterData(context: PokeroleActorSheetData) {
		return new Object();
	}

	/**
	 * Organize and classify Items for Character sheets.
	 *
	 * @param {Object} actorData The actor to prepare.
	 *
	 * @return {undefined}
	 */
	_prepareMonItems(context: PokeroleActorSheetData) {
		// Initialize containers.
		//effects == status?
		const gear = [];
		const features = [];
		const moves = [];
		const types = [];
		const accessories = [];
		// var ability;
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

		// Iterate through items, allocating to containers
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
		// 	if (i.type === 'ability') {
		// 		ability = i;
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
		// context.moves = moves;
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
	protected _injectHTML(html: JQuery<HTMLElement>): void {
		//put that in before it gets loaded
		this._injectSvg(html);
		super._injectHTML(html);
	}
	protected _replaceHTML(element: JQuery<HTMLElement>, html: JQuery<HTMLElement>): void {
		this._injectSvg(html);
		super._replaceHTML(element, html);
	}
	private _currentSvg?: HTMLElement;
	private _injectSvg(html: JQuery) {
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
				thisRef._currentSvg = thisRef._cachedPristineSvg!.cloneNode(true) as HTMLElement;
				embed.replaceWith(thisRef._currentSvg);
				thisRef._initSvg();//since we had to delay it
			};
			//stupid race condition... run it later
			embed.addEventListener("load", initFunc);
			return;
		}
		this._currentSvg = this._cachedPristineSvg.cloneNode(true) as HTMLElement;
		embed.replaceWith(this._currentSvg);
	}
	private _initSvg() {
		var svg = this._currentSvg;
		if (!svg) {
			//try again later
			return;
		}
		//find my items...
		var items = svg.getElementsByClassName("svg-item");
		//now we need to figure out what is what
		this._assignSvgObjects(Array.from(items) as SVGElement[], svg.parentElement!);
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
	_assignSvgObjects(svgItems: SVGElement[], domParent: HTMLElement) {
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
						var dataObj = <PlayerActorData><unknown>this.actor.system;
						setActorStat(dataObj, statName, target);
						this._setStatSvg0(dataObj, statName, dotInfo);
						//send update information
						// var dataPath = `data.${statName}`;
						var dataPartial = {} as any;
						dataPartial['stats'] = {} as any;
						dataPartial['stats'][statName] = target;
						this.actor.update({
							system: dataPartial
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
				case "hp":
					this._moveElement(item, "lblHp", domParent);
					break;
				case 'will':
					this._moveElement(item, "lblWill", domParent);
					break;
				case 'nature':
					this._moveElement(item, 'cboNature', domParent);
					break;
				case 'confidence':
					this._moveElement(item, 'lblConfidence', domParent);
					break;
				//#region Pokemon
				case 'battleCount':
					this._moveElement(item, "nudBattleCount", domParent);
					break;
				case 'victoryCount':
					this._moveElement(item, "nudVictoryCount", domParent);
					break;
				case 'primaryAccessory':
					this._moveElement(item, 'txtPrimaryAccessory', domParent);
					break;
				case 'item':
					this._moveElement(item, 'txtItem', domParent);
					break;
				case 'defenses':
					this._moveElement(item, 'lblDefense', domParent);
					break;
				case 'name':
					this._moveElement(item, 'lblName', domParent);
					break;
				case 'ability':
					this._moveElement(item, 'lblAbility', domParent);
					break;
				//#endregion
			}
		}
	}
	/**
	 * Move the item specified by {@link htmlId} over {@link item}. The found element is returned. If the element
	 * could not be found, null will be returned and {@link item}'s fill will be set to red
	 * @param item 
	 * @param htmlId 
	 * @param domDest 
	 * @returns 
	 */
	private _moveElement(item: SVGElement, htmlId: string, domDest: HTMLElement) : HTMLElement | null {
		var display = $(`.${htmlId}`, this.element).get(0) as HTMLElement;
		if (!display) {
			//could not find!!!
			item.style.fill = "red";
			return null;
		}
		this._moveOver(display, item, domDest);
		return display;
	}
	/**
	 * Move {@link item} such that it is over {@link dest}
	 * @param item 
	 * @param dest 
	 */
	private _moveOver(item: HTMLElement, dest: SVGElement, domDest: HTMLElement) {
		//run some maths!!!
		//these are relative the the browser window, so we need to get relative values
		var rects = domDest.getClientRects();//this.element[0]!.getClientRects();
		//there should only be one?
		console.assert(rects.length == 1);
		var windowRect = rects[0]!;
		rects = dest.getClientRects();
		//there should only be one?
		console.assert(rects.length == 1);
		var destRect = rects[0]!;
		var x = destRect.x;
		var y = destRect.y;
		x -= windowRect.x;
		y -= windowRect.y;
		item.remove();
		domDest.append(item);
		item.style.position = "absolute";
		item.style.top = y + "px";
		item.style.left = x + "px";

		item.style.height = destRect.height + "px";
		item.style.width = destRect.width + "px";

		// var
		// 	// var rootParent = svgRoot.parentElement!;
		// 	hpDisplay.remove();
		// // item.appendChild(hpDisplay);
		// rootParent.appendChild(hpDisplay);
		// // //move it into position...
		// hpDisplay.style.position = "absolute";
		// hpDisplay.style.top = "10px";
		// hpDisplay.style.left = "10px";
		// var x = 0;
		// var y = 0;
		// var parent = dest;


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
			case "perform":
			case "tough":
			case "cool":
			case "beauty":
			case "clever": 
			case "cute":
				return { min: 1, max: 5 };
			default:
				return { min: 0, max: 5 };
		}
	}
	private _setStatSvg(context: PokeroleActorSheetData, stat: string, dotInfo: DotInfo) {
		if (context.actor.type === POKEROLE.ActorTypes.rival) {
			return;
		}
		this._setStatSvg0(context.system as PlayerActorData, stat, dotInfo);
	}
	private _setStatSvg0(data: PlayerActorData, stat: string, dotInfo: DotInfo) {
		// just double checking, these should all be paths
		//supporting circles since I want to change them to that eventually...
		for (var dot of dotInfo.dots) {
			if (dot.tagName !== "path" && dot.tagName !== "circle") {
				//svg set up wrong
				throw new Error(`Named dot (${dot.id}) was not an svg path!`)
			}
		}
		var num = getActorStat(data, stat);
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
		this._initSvg();

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

