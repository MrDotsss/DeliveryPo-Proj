//can be added to main quest


VAR trustLvl = 0

-> start

=== start ===
npc: Oh hello, how can I help you?
    * [Ask about cult]
        -> hint
    * [Leave]
        you: Oh nothing, just passing by.
        -> END


=== hint ===
{trustLvl < 0: -> evade}

npc: I believe the cults are hiding behind that stairs.
npc: But, be careful they have these weird people that looks like monsters.
you: Thank you, I'll take that in mind.
-> END

=== evade ===
npc: Cult? What do you mean?
you: Nevemind.
-> END