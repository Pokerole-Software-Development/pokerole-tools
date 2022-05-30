import {v5 as Uuid} from 'uuid';
export class InvalidBuildError extends Error{
}
export class DataId  {
	//This is a nullable int since "0" is a valid db id and we don't want to have to worry about that issue
	public readonly dbId: number | undefined;
	public readonly uuid: typeof Uuid;
	constructor(uuidIn: typeof Uuid, dbId: number | undefined){
		this.uuid = uuidIn;
		this.dbId = dbId;
	}
	public toString() : String{
		return `Uuid: ${this.uuid}, DbId: ${this.dbId}`;
	}
	public static Builder = class{
		private _dbId : number | undefined;
		public get dbId() : number | undefined {
			return this._dbId;
		}
		public set dbId(v : number | undefined) {
			this._dbId = v;
		}
		
		private _uuid : typeof Uuid | undefined;
		public get uuid() : typeof Uuid | undefined {
			return this._uuid;
		}
		public set uuid(v : typeof Uuid | undefined) {
			this._uuid = v;
		}
		constructor(existing: DataId | undefined){
			if (existing !== undefined){
				this._uuid = existing.uuid;
				this._dbId = existing.dbId;
			}
		}
		public build() : DataId{
			if (this._uuid === undefined){
				throw new InvalidBuildError("UUID was not set");
			}
			return new DataId(this._uuid, this._dbId);
		}
	}
}
export class 
