namespace pokerole{
	export interface IItemBuilder{
		isValid() : boolean;
		build() : unknown;
		/**
		 * Which propertes of this instance are not set, but should be set. Generally for debugging.
		 */
		missingItems() : Array<String>;
		// builderType() : Type Typescript doesn't support reflection
		values() : Array<[String, unknown]>;
	}

	export abstract class ItemBuilder<T> implements IItemBuilder{
		/**
		 * Whether or not all of the required Properites of this instance are set to build a new instance of T.
		 * @method build will throw an exception if this returns false.
		 */
		public abstract isValid(): boolean;
		/**
		 * Build an instance of T from this builder
		 */
		public abstract build(): T;
		public abstract missingItems() : Array<String>;
		public abstract values(): [String, unknown][];
	}
}
