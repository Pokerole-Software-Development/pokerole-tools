/**
 * Define a set of template paths to pre-load
 * Pre-loaded templates are compiled and cached for fast access when rendering
 * @return {Promise}
 */
 export const preloadHandlebarsTemplates = async function() {
  return loadTemplates([

    // Actor partials.
    "systems/Pokerole/templates/actor/parts/actor-features.html",
    "systems/Pokerole/templates/actor/parts/actor-items.html",
    "systems/Pokerole/templates/actor/parts/actor-effects.html",
    // "systems/Pokerole/templates/actor/parts/Mon-card.svg",
    // "systems/Pokerole/templates/actor/parts/move-block.svg",
    // "systems/Pokerole/templates/actor/parts/Rival with image blockers.svg",
    // "systems/Pokerole/templates/actor/parts/Trainer-card.svg",
  ]);
};
