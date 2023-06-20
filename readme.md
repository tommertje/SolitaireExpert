player clicks/taps on a card or in the air => Unity3D triggers OnMouseDown as well as **OnMouseDrag** once.

player holds the mouse down => Unity3D triggers OnMouseDrag 'every frame' at 30 Frames Per Second.

player drags that card => Unity3D triggers first OnMouseDrag and then Update 'every frame' at 30 Frames Per Second.

player drags that card near something => Unity3D triggers OnTriggerEnter for that card **as well as** for that something

player drags that card away from that something => Unity3D triggers OnTriggerExit for that card **as well as** for that something

player stops dragging that card => Unity3D triggers OnMouseUp


gameController setup up the cards => Unity3D triggers OnTriggerEnter for two cards directly below each other


in OnMouseDown we check CanBePickedUp and if card is in a stack.

in OnMouseDrag we move the card and the cards below.
the goal of the first call of AddToMovingStack it to create the moving stack; the other, recursive calls are to add 1 card.


in OnTriggerEnter and OnTriggerExit we set possible new positions of the card and we make a list of CardsAbove and CardsBelow the card = stack.

in OnMouseUp we check CanBeDropped and we DropCard.


each card in each column forms a (virtual) stack of ordered cards; the stacks consists of either

1. itself

2. itself and 1 or more CardsAbove

3. itself and 1 or more CardsBelow

4. itself and 1 or more CardsAbove and 1 or more CardsBelow


player can drag that stack as cardsInMovingStack

player can also drag part that stack = cardsInMovingStack = the CardsBelow


