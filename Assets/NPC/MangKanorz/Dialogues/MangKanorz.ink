//alias: Old Man
//npc: Mang Kanorz

//Favor Quest: Guide the Old
//Guide Mang Kanorz to the stairs.
VAR questFinished = false
VAR accepted = false

//Address: Spiral Staircase
VAR tooFar = false

{questFinished == true: -> finished }
{accepted == true: -> check | -> start}

=== start ===
alias: Oh, young lady...
you: What's the matter?
alias: I think I'm lost.
alias: Do you know where are the spiral stairs?
    * you: Yes, I think so.
        -> ask
    * you: I just got here
        -> decline
-> END

=== decline ===
alias: Too bad.
-> END

=== ask ===
alias: Can you help me go to the stairs?
    * Yes, of course.
        #quest
        npc: Oh thank you so much.
        npc: That was nice, I'm Kanorz
        npc: You can call me, Mang Kanorz.
        -> END
    * I have something to do.
        npc: Oh, too bad -> END
-> END

=== check ===
{tooFar == true : npc: Hey, wait for me. | npc: Are we there yet? }
-> END

=== finished ===
npc: Thank you so much!
you: You are welcome
-> END





