//Bruv
//Delivery NPC

VAR trustLvl = 0

-> decide


=== decide ===
{trustLvl == 0:
    -> main
}
-> END

=== main ===
you: Hello, are you bruv?
alias: Yes, I am.
npc: What is it?
    * [Ask why is he capsule shape]
        -> answerToCapsule
    * [There is delivery for him]
        -> delivery
    * [Just walk away]
        npc: what?
        npc: rude...
        ~trustLvl -= 0.5
        -> END

=== answerToCapsule ===
~trustLvl -= 0.3
npc: Hey, you don't even have a body.
npc: Just say, what do you want?
    * [There is delivery for him]
        -> delivery
    * [Just walk away]
        npc: what?
        npc: rude...
        ~trustLvl -= 0.5
        -> END

=== delivery ===
you: There's a delivery for you.
you: It's a, capsule bed?
npc: Yes that's mine.
    * [Give parcel]
        #deliver
        npc: Thank you
        you: Can I ask to take you proof of delivery?
        npc: Sure sure...
        ~trustLvl += 0.5
        -> END
    * [Just walk away]
        npc: what?
        npc: rude...
        ~trustLvl -= 0.5
        -> END

