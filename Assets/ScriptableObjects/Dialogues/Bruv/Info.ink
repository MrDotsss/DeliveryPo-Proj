//Bruv
//Talking me

VAR trustLvl = 0

-> decide

=== decide ===
{trustLvl != 0:
    {trustLvl > 0: -> trusted|-> untrusted}
  - else:
    npc: Nothing to say.
    -> END
}


=== trusted ===
npc: Oh hello there. 
npc: I trust you.
 * [Walk away]
    ~trustLvl -= 0.5
    npc: Oh not anymore...
    -> END
 * [Greet back]
    ~trustLvl += 0.1
    you: Hello, thank you for trusting me.
    npc: You are welcome...
    -> END

=== untrusted ===
npc: Go awayy...
    * [Apologize]
        ~trustLvl += 0.05
        you: I'm very sorry.
        npc: Well then, you are nice.
        -> END
    * [Snob]
        npc: Yeah, that's right. Get out of here.
    -> END