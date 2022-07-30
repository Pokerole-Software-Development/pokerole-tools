// Import document classes.
import { PokeroleActor } from "./documents/actor.js"
import { PokeroleItem } from "./documents/item.js"
// Import sheet classes.
import { PokeroleActorSheet } from "./sheets/actor-sheet.js"
import { PokeroleItemSheet } from "./sheets/item-sheet.js"
// Import helper/utility classes and constants.
import { preloadHandlebarsTemplates } from "./helpers/templates.js"
import { POKEROLE } from "./helpers/config.js"

/* -------------------------------------------- */
/*  Init Hook                                   */
/* -------------------------------------------- */

Hooks.once('init', async function () {
  console.log(`Pokérole | Initializing the Pokérole Game System\n${POKEROLE.ASCII}`);
  // throw new Error("This is a test");
  // Add utility classes to the global game object so that they're more easily
  // accessible in global contexts.
  //initialized before 'init'
  var theGame = game as Game;
  theGame.Pokerole = {
    PokeroleActor,
    PokeroleItem,
    // rollItemMacro
  };
  theGame.settings.register('Pokerole', 'UseInsightForSpecialDefense',
    {
      name: 'Use insight for special defense',
      hint: 'Whether or not to use insight for special defense instead of vitality',
      type: Boolean,
      onChange: val=> {
        POKEROLE.UseInsightForSpecialDefense = val.valueOf();
      }
    }
  );

  // Add custom constants for configuration.
  CONFIG.POKEROLE = POKEROLE;

  /**
   * Set an initiative formula for the system
   * @type {String}
   */
  CONFIG.Combat.initiative = {
    formula: "@stat.dexterity + @stat.alert + 1d6",
    decimals: 2
  };

  // Define custom Document classes
  CONFIG.Actor.documentClass = PokeroleActor;
  CONFIG.Item.documentClass = PokeroleItem;

  // Register sheet application classes
  Actors.unregisterSheet("core", ActorSheet);
  Actors.registerSheet("Pokerole", PokeroleActorSheet, { makeDefault: true });
  Items.unregisterSheet("core", ItemSheet);
  Items.registerSheet("Pokerole", PokeroleItemSheet, { makeDefault: true });

  // Preload Handlebars templates.
  return preloadHandlebarsTemplates();
});

/* -------------------------------------------- */
/*  Handlebars Helpers                          */
/* -------------------------------------------- */

// If you need to add Handlebars helpers, here are a few useful examples:
Handlebars.registerHelper('concat', function() {
  var outStr = '';
  for (var arg in arguments) {
    if (typeof arguments[arg] != 'object') {
      outStr += arguments[arg];
    }
  }
  return outStr;
});

Handlebars.registerHelper('toLowerCase', function(str) {
  return str.toLowerCase();
});
// Handlebars.re

/* -------------------------------------------- */
/*  Ready Hook                                  */
/* -------------------------------------------- */

Hooks.once("ready", async function() {
  // Wait to register hotbar drop hook on ready so that modules could register earlier if they want to
  // Hooks.on("hotbarDrop", (bar, data, slot) => createItemMacro(data, slot));
});

/* -------------------------------------------- */
/*  Hotbar Macros                               */
/* -------------------------------------------- */


let todo = `
/**
 * Create a Macro from an Item drop.
 * Get an existing item macro if one exists, otherwise create a new one.
 * @param {Object} data     The dropped data
 * @param {number} slot     The hotbar slot to use
 * @returns {Promise}
 */
async function createItemMacro(data, slot) {
  if (data.type !== "Item") return;
  if (!("data" in data)) return ui.notifications.warn("You can only create macro buttons for owned Items");
  const item = data.data;

  // Create the macro command
  const command = \`game.Pokerole.rollItemMacro("$ {item.name}");\`;
  let macro = game.macros.find(m => (m.name === item.name) && (m.command === command));
  if (!macro) {
    macro = await Macro.create({
      name: item.name,
      type: "script",
      img: item.img,
      command: command,
      flags: { "Pokerole.itemMacro": true }
    });
  }
  game.user.assignHotbarMacro(macro, slot);
  return false;
}

/**
 * Create a Macro from an Item drop.
 * Get an existing item macro if one exists, otherwise create a new one.
 * @param {string} itemName
 * @return {Promise}
 */
function rollItemMacro(itemName) {
  const speaker = ChatMessage.getSpeaker();
  let actor;
  if (speaker.token) actor = game.actors.tokens[speaker.token];
  if (!actor) actor = game.actors.get(speaker.actor);
  const item = actor ? actor.items.find(i => i.name === itemName) : null;
  if (!item) return ui.notifications.warn(\`Your controlled Actor does not have an item named $ {itemName}\`);

  // Trigger the item roll
  return item.roll();
}
`
