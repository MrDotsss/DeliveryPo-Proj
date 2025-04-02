//alias: Gloomy Guy
//npc: Pida

// Favor Quest: Lost Pizza
// Find lost pizza of Pida
// Item: Pizza

VAR questFinished = false
VAR accepted = false

VAR trustGrade = 0

{questFinished == false: -> Favor | -> Finished}

=== Favor ===
{trustGrade < 0 && accepted == false: -> EVADE }
{accepted == true && questFinished == false: -> CLUE}
alias: huhuhu
    * you: Why are you crying?
        -> QUESTION
    * [Ignore]
        npc: please help me!
        -> END

=== CLUE ===
npc: I think I dropped it near the stairs.
-> END

=== QUESTION ===
alias: I dropped my pizza somehere!
npc: I'm Pida.
npc: Please help me find it T^T
    * you: Alright.
        #quest // register this quest to the player
        npc: Thank you so much.
        ~accepted = true
        -> END
    * you: Mind your own problem.
        npc: Huhu T^T
        -> END


=== Finished ===
npc: Oh thank you so much!
-> END

=== EVADE ===
alias: Don't talk to me...
-> END