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
