// alias: Young Man
// npc: Mr. Javier

// DAY 1
// First Interaction w/ MC

//Delivery: Mr. Javier
//Item: Japanese Dictionary
//Address: Center of the Plane

VAR trustLvl = 0


You ring the doorbell.
The door opened. // customer neutral face
A young man came out of the house.
    * [Hello, I'm the new deliveryman in this area.]
    you: Hello, I'm the new deliveryman in this area.
        alias: Where's the previous deliveryman? // Young Man:
        -> START1
    * [Hey, you have a delivery.]
    you: Hey, you have a delivery.
        alias: Where's the previous deliveryman? // Young Man:
        -> START1
    * [Good Morning! I'm here to deliver your parcel.]
    You: Good Morning! I'm here to deliver your parcel.
    ~ trustLvl += 0.1
    // ( Trust +1 )
        -> START2


=== START1 ===
* [I was told that he resigned. Looks like the company hired me as his replacement.]
you: I was told that he resigned. Looks like the company hired me as his replacement.
    alias: I see, so that's what happened. // Young Man:
    // customer neutral face
    {trustLvl == 0.1:
    -> ASKNAME2
  - else:
    -> ASKNAME1
}
-> DONE
    * [I dunno. Why do you care?]
    you: I dunno. Why do you care?
        alias: What? I was just curious. // Young Man:
        // customer surprised face
        alias: You don't have to be rude... // Young Man:
        // customer sad face
        ~ trustLvl -= 0.1
        // ( Trust -1 )
            ** [(Apologize)]
            -> SORRY
            ** [(Ask about his name.)]
            -> ASKNAME3
-> DONE


=== START2 ===
// customer smiles
alias: Good morning to you as well! // Young Man:
alias: Where's the usual deliveryman? // Young Man:
    -> START1
-> DONE


=== ASKNAME1 ===
* [(Ask about his name.)]
you: Are you Mr. Luis Javier?
    npc: That's me, yes. // Mr. Javier: 
    -> GIVEPARCEL1


=== ASKNAME2 ===
* [(Ask about his name.)]
you: Are you Mr. Luis Javier?
    npc: Yes! That's me. // Mr. Javier: 
    -> GIVEPARCEL2


=== ASKNAME3 ===
you: Are you Mr. Luis Javier?
    npc: Yes... // Mr. Javier: 
    -> GIVEPARCEL3


=== SORRY ===
you: I'm sorry, I didn't mean to say that.
    alias: It's fine. Just be mindful next time. // Young Man:
    // customer slightly frowns
    -> ASKNAME1
    

=== GIVEPARCEL1 ===
#deliver // give parcel to the npc
* [Here's your parcel. It says "Japanese Dictionary". Is that right?] //gives parcel
you: Here's your parcel. It says "Japanese Dictionary". Is that right?
    -> DIAL4
* [Just take your parcel.] //gives parcel
you: Just take your parcel.
    npc: Woah, ok ok. // Mr. Javier: 
    // customer sad face
    -> OUTRO
-> DONE


=== GIVEPARCEL2 ===
#deliver
* [Here's your parcel. It says "Japanese Dictionary". Is that right?] //gives parcel
you: Here's your parcel. It says "Japanese Dictionary". Is that right?
    -> DIAL4
* [Here, just take the parcel. Sorry, I don't really have much time to talk.] //gives parcel
you: Here, just take the parcel. Sorry, I don't really have much time to talk.
    npc: Oh, sorry. I just got curious. // Mr. Javier: 
    // customer apologetic face
    -> OUTRO
-> DONE


=== GIVEPARCEL3 ===
#deliver
* [Here's your parcel. "Japanese Dictionary" right?] //gives parcel
you: Here's your parcel. "Japanese Dictionary" right?
            npc: Yeah, thanks. // Mr. Javier: 
            // customer neutral face
            -> OUTRO
-> DONE
* [Just take your parcel.] //gives parcel
you: Just take your parcel.
    npc: Wow... I can't believe you can be like this to customers. // Mr. Javier: 
    // customer angry face
    ~ trustLvl -= 0.1
    // ( Trust +1 )
    -> OUTRO
-> DONE


=== DIAL4 ===
// customer smiles widely
npc: Yes, thank you! I've been waiting for this! // Mr. Javier: 
npc: You see, I'm writing a new novel and I wanted to add Japanese elements to it. // Mr. Javier: 
    * [So, you're a writer. I'm kinda interested.]
    you: So, you're a writer. Let me know your progress next time!
        npc: Really?! You got it! // Mr. Javier: 
        npc: I'll show you how much I've improved by then! // Mr. Javier: 
        // ( Trust +2 )
        ~ trustLvl += 0.2
        -> OUTRO
    * [That seems fun. Enjoy using it then.]
    you: That seems fun. Enjoy using it then.
        npc: Thanks! // Mr. Javier: 
        // customer smiles
        -> OUTRO
    * [Hmm, not interested sorry.]
    you: Hmm, not interested sorry.
        npc: Oh, too bad then. // Mr. Javier: 
        // customer slightly sad face
        -> OUTRO
-> DONE


=== OUTRO ===
{trustLvl <= -0.2: -> OUTRO1}
{trustLvl > 0.1: -> OUTRO4}
{trustLvl == 0.1: -> OUTRO3} 
{trustLvl > -0.2: -> OUTRO2}
-> DONE


=== OUTRO1 ===
* [(Ask for POD)]
you: I need to take your picture for Proof of Delivery (POD).
    npc: ... // Mr. Javier: 
    npc: Sure, just make it quick... // Mr. Javier: 
    // customer frowns
    
-> END


=== OUTRO2 ===
* [(Ask for POD)]
you: I also need to take your picture for Proof of Delivery (POD).
    npc: Sure. // Mr. Javier: 
    // customer neutral face
    
-> END

=== OUTRO3 ===
* [(Ask for POD)]
you: I also need to take your picture for Proof of Delivery (POD).
    npc: Sure! // Mr. Javier: 
    // customer smiles
    
-> END

=== OUTRO4 ===
* [(Ask for POD)]
you: Can I also please take your picture for Proof of Delivery (POD)?
    npc: Sure! Go on! // Mr. Javier: 
    //customer smiles widely
    
-> END

