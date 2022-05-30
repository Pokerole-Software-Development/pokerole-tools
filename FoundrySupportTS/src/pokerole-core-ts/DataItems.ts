// import {v5 as Uuid} from 'uuid';
type Uuid = String;
export class InvalidBuildError extends Error{
}
namespace pokerole{
	export class DataId  {
		//This is a nullable int since "0" is a valid db id and we don't want to have to worry about that issue
		public readonly dbId: number | undefined;
		public readonly uuid: Uuid;
		public constructor(uuidIn: Uuid, dbId: number | undefined){
			this.uuid = uuidIn;
			this.dbId = dbId;
		}
		public toString() : String{
			return `Uuid: ${this.uuid}, DbId: ${this.dbId}`;
		}
	}
	/**
	 * A reference to an item of type T
	 */
	export class ItemReference<T>{
		public readonly dataId : DataId;
		public readonly displayName : String | undefined;
		/**
		 * Hint to the caller about whether or not this ItemReference references something built-in like the Normal Type
		 */
		public readonly buildIn : boolean = false;
		public constructor(dataId: DataId, displayName: String | undefined){
			this.dataId = dataId;
			this.displayName = displayName;
		}
		public toString() : String{
			if (this.displayName === undefined){
				return this.dataId.toString();
			}
			return `DisplayName = ${this.displayName}, ${this.dataId}`;
		}
		// public static Builder = class<T> extends ItemBuilder<ItemReference<T>>{
		// 	public isValid(): boolean {
		// 		throw new Error('Method not implemented.');
		// 	}
		// 	public build(): ItemReference<T> {
		// 		throw new Error('Method not implemented.');
		// 	}
		// 	public missingItems(): String[] {
		// 		throw new Error('Method not implemented.');
		// 	}
		// 	public values(): [String, unknown][] {
		// 		throw new Error('Method not implemented.');
		// 	}

		// }
	}
}
