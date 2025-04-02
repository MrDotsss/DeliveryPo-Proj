//alias: Huge guy
//name: Bona Mil

//Favor Quest: Gorbir Munchies
// Bring 2 burgers to Bona
// Item: Burgers

VAR questFinished = false
VAR accepted = false

//vars
VAR burgersGave = 0 // checker of number of burgers gave
VAR givenPizza = false // if the player accepts pizza

-> start

=== start ===
{questFinished == true: -> finished}
{accepted == true: -> check}

alias: Hello good ma'am. Can you help me?
    * you: Yes, sure what is it?
        #quest // register quest to the manager
        alias: Thank you. I'm Bona Mil by the way, nice to meet you?
        you: Marimar
        npc: Nice to meet you, Marimar.
        npc: You see, my sitter left me and I want some bargers.
        you: Burgers?
        npc: Yes-yes Bargers!
        you: Alright, I got you.
        npc: Thank you, Marimar.
        -> check
    * you: Mind your own business.
        alias: Sad.
        -> END

=== check ===
{burgersGave != 0:
    {burgersGave == 1: npc: Thank you! One more to go.}
    {burgersGave == 2: -> finished }
  - else:
    npc: I need two bargers please.
}
-> END


=== finished ===
npc: Mmmm.. thank you so much, Marimar.
{givenPizza == false: -> give }
-> END

=== give ===
npc: I would like to give you this, a Pizza.
    * [Accept it]
        #give // add pizza and equip to the player inventory
        ~ givenPizza = true
        you: Oh thank you. This looks tasty.
        -> END
    * [Discard it]
        you: Actually, I feel like you need it more than I do.
        -> END