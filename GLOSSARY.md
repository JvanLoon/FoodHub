# FoodHub

A shared library of Recipes that Users browse, plan into a Meal Plan and turn into a shopping list. Content added by Users is moderated before it becomes visible to others.

## Language

### Content

**Recipe**:
A named dish made of Recipe Items, authored by one User.
_Avoid_: Dish, meal

**Ingredient**:
An entry in the shared catalog of ingredient names that Recipe Items refer to.
_Avoid_: Product, item

**Recipe Item**:
One line of a Recipe: an Ingredient plus an amount and a unit.
_Avoid_: Ingredient line, recipe ingredient, line

### Review

**Review**:
The process in which a Moderator decides whether a Pending Recipe becomes Approved or is rejected.
_Avoid_: Moderation, vetting

**Pending**:
The state of a Recipe (or Ingredient) that has not passed Review. Only its author can see it. A Recipe returns to Pending whenever its name or Recipe Items change.
_Avoid_: Unreviewed, draft, unapproved

**Approved**:
The state of a Recipe (or Ingredient) that has passed Review. Visible to every User.
_Avoid_: Reviewed, published, public

**Changed Recipe Item**:
A Recipe Item added or edited since its Recipe was last Approved; the Reviewer must look at these specifically. A Recipe cannot be approved while any remain unresolved.
_Avoid_: Dirty line, pending line

**Reject**:
Reviewing a Pending target by permanently deleting it. No reason and no reviewer is recorded.
_Avoid_: Decline, deny

**First Submission**:
A Pending Recipe that has never been Approved, as opposed to an edit of an already-Approved Recipe.
_Avoid_: New recipe

### People

**User**:
A person with an account who can author Recipes and keep a Meal Plan.

**Moderator**:
A role that may Review content and edit any Recipe or Ingredient.

**Admin**:
A role with every Moderator ability plus user, role and data-import/export management.
_Avoid_: Administrator

### Planning

**Meal Plan**:
A User's own record of which Recipes they intend to eat on which days.
_Avoid_: Meal calendar, schedule

**Meal Plan Entry**:
One Recipe planned on one day in one User's Meal Plan.

**Calendar**:
The week and month views through which a Meal Plan is shown and edited; a view, not a separate concept.

**Randomize**:
Filling selected days of a Meal Plan with Recipes chosen at random, optionally biased toward wanted Ingredients.

**Blacklisted Recipe**:
A Recipe a User has chosen never to see again. It is never retrieved for that User, wherever Recipes are retrieved. Blacklisting removes the User's Meal Plan Entries for that Recipe, and the User is warned first. Reversed by un-blacklisting.
_Avoid_: Excluded recipe, hidden recipe, whitelist
