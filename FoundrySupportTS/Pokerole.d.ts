/// <reference path="src/helpers/config.ts"/>
/// <reference path="./src/documents/actor"/>



// import { PokeroleActorData } from "./src/documents/actor";

interface Game{
	//define later
	Pokerole: any;
}
interface User{
	// /**
	//  * Selected tokens
	//  */
	// targets: Set<Token>
}
interface CONFIG{
	//define later
	POKEROLE: POKEROLE;
}
interface Actor{//<T> extends Actor{
	/**
	 * V10 data object
	 */
	system: SystemDataField;// & PokeroleActorData;
	/**
	 * Deprecated in v10. Use 'system' instead
	 */
	// data: void;Note: we cannot do that yet. The template thing defines various props by reference through this
}
interface Item{//<T> extends Item{
	/**
	 * V10 data object
	 */
	system: SystemDataField;// & PokeroleItemData;
	/**
	 * Deprecated in v10. Use 'system' instead
	 */
	// data: void;Note: we cannot do that yet. The template thing defines various props by reference through this
}
// namespace ItemSheet{
// 	interface Data {
// 		/**
// 		 * V10 data object
// 		 */
// 		system: Object;
// 	}
// }
/**
 * @deprecated Not used in V10
 */
interface ActorData{
	/**
	 * Deprecated in v10. Use 'system' instead
	 */
	// data: void;Note: we cannot do that yet. The template thing defines various props by reference through this
}
/**
 * @deprecated Not used in V10
 */
interface ItemData {
	/**
	 * Deprecated in v10. Use 'system' instead
	 */
	// data: void;Note: we cannot do that yet. The template thing defines various props by reference through this
}
// interface PokeroleActor{
// 	readonly system: PokeroleActorData & SystemDataField;
// 	readonly itemTypes: {
// 		ability: AbilityItemData[],
// 		accessory: AccessoryItemData[],
// 		dexentry: object[],//define that later...
// 		item: object[],//define that later...
// 		move: MoveItemData[],
// 		type: string[]
// 	}
// }
// interface PokeroleItem {
// 	readonly system: PokeroleItemData & SystemDataField;
// }
/**
 * Mostly a marker interface so we know what the heck is and isn't from a system data field
 */
interface SystemDataField{// extends ObjectField{
	toObject(source: false): object;
}

//just declaring it exists... From foundry internals
/**
 * A representation of a color in hexadecimal format.
 * This class provides methods for transformations and manipulations of colors.
 */
interface Color extends Number {

	/**
	 * A CSS-compatible color string.
	 * An alias for Color#toString.
	 * @type {string}
	 */
	get css(): string;

	/* ------------------------------------------ */

	/**
	 * The color represented as an RGB array.
	 * @type {[number, number, number]}
	 */
	get rgb(): [number, number, number];

	/* ------------------------------------------ */

	/**
	 * The numeric value of the red channel between [0, 1].
	 * @type {number}
	 */
	get r(): number;

	/* ------------------------------------------ */

	/**
	 * The numeric value of the green channel between [0, 1].
	 * @type {number}
	 */
	get g(): number;

	/* ------------------------------------------ */

	/**
	 * The numeric value of the blue channel between [0, 1].
	 * @type {number}
	 */
	get b(): number;

	/* ------------------------------------------ */

	/**
	 * The maximum value of all channels.
	 * @type {number}
	 */
	get maximum(): number;

	/* ------------------------------------------ */

	/**
	 * The minimum value of all channels.
	 * @type {number}
	 */
	get minimum(): number;

	/* ------------------------------------------ */

	/**
	 * Get the value of this color in little endian format.
	 * @type {number}
	 */
	get littleEndian(): number;

	/* ------------------------------------------ */

	/**
	 * The color represented as an HSV array.
	 * Conversion formula adapted from http://en.wikipedia.org/wiki/HSV_color_space.
	 * Assumes r, g, and b are contained in the set [0, 1] and returns h, s, and v in the set [0, 1].
	 * @type {[number, number, number]}
	 */
	get hsv(): [number, number, number];

	/* ------------------------------------------ */
	/*  Color Manipulation Methods                */
	/* ------------------------------------------ */

	/** @override */
	toString(radix): string;

	/* ------------------------------------------ */

	/**
	 * Test whether this color equals some other color
	 * @param {Color|number} other  Some other color or hex number
	 * @returns {boolean}           Are the colors equal?
	 */
	equals(other): boolean;

	/* ------------------------------------------ */

	/**
	 * Get a CSS-compatible RGBA color string.
	 * @param {number} alpha      The desired alpha in the range [0, 1]
	 * @returns {string}          A CSS-compatible RGBA string
	 */
	toRGBA(alpha): number;

	/* ------------------------------------------ */

	/**
	 * Mix this Color with some other Color using a provided interpolation weight.
	 * @param {Color} other       Some other Color to mix with
	 * @param {number} weight     The mixing weight placed on this color where weight is placed on the other color
	 * @returns {Color}           The resulting mixed Color
	 */
	mix(other, weight): Color;

	/* ------------------------------------------ */

	/**
	 * Multiply this Color by another Color or a static scalar.
	 * @param {Color|number} other  Some other Color or a static scalar.
	 * @returns {Color}             The resulting Color.
	 */
	multiply(other): Color;

	/* ------------------------------------------ */

	/**
	 * Add this Color by another Color or a static scalar.
	 * @param {Color|number} other  Some other Color or a static scalar.
	 * @returns {Color}             The resulting Color.
	 */
	add(other): Color;

	/* ------------------------------------------ */

	/**
	 * Subtract this Color by another Color or a static scalar.
	 * @param {Color|number} other  Some other Color or a static scalar.
	 * @returns {Color}             The resulting Color.
	 */
	subtract(other): Color;

	/* ------------------------------------------ */

	/**
	 * Max this color by another Color or a static scalar.
	 * @param {Color|number} other  Some other Color or a static scalar.
	 * @returns {Color}             The resulting Color.
	 */
	maximize(other): Color;

	/* ------------------------------------------ */

	/**
	 * Min this color by another Color or a static scalar.
	 * @param {Color|number} other  Some other Color or a static scalar.
	 * @returns {Color}             The resulting Color.
	 */
	minimize(other): Color

	/* ------------------------------------------ */
	/*  Iterator                                  */
	/* ------------------------------------------ */

	/**
	 * Iterating over a Color is equivalent to iterating over its [r,g,b] color channels.
	 * @returns {Generator<number>}
	 */
	iterator(): Generator<number>;

	/* ------------------------------------------ */
	/*  Factory Methods                           */
	/* ------------------------------------------ */

	/**
	 * Create a Color instance from an RGB array.
	 * @param {null|string|number|number[]} color A color input
	 * @returns {Color|NaN}                       The hex color instance or NaN
	 */
	static from(color): Color | NaN;

	/* ------------------------------------------ */

	/**
	 * Create a Color instance from a color string which either includes or does not include a leading #.
	 * @param {string} color                      A color string
	 * @returns {Color}                           The hex color instance
	 */
	static fromString(color): Color;

	/* ------------------------------------------ */

	/**
	 * Create a Color instance from an RGB array.
	 * @param {[number, number, number]} rgb      An RGB tuple
	 * @returns {Color}                           The hex color instance
	 */
	static fromRGB(rgb): Color

	/* ------------------------------------------ */

	/**
	 * Create a Color instance from an HSV array.
	 * Conversion formula adapted from http://en.wikipedia.org/wiki/HSV_color_space.
	 * Assumes h, s, and v are contained in the set [0, 1].
	 * @param {[number, number, number]} hsv      An HSV tuple
	 * @returns {Color}                           The hex color instance
	 */
	static fromHSV(hsv): Color
}
