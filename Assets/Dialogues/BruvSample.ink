INCLUDE GlobalStory.ink

{trustLevel >= 0: -> start | -> away }

=== start ===
Hello there driver! Welcome to our village!
* [Who are you?] -> ask
* [Walk away] -> away


=== ask ===
I'm bruv, I live near the church.
    * [Nice to meet you.]
        ~ trustLevel += 1
        It's nice to meet you too! -> END
    * [You are creepy] -> away


=== away ===
Rude. Bye then.
~ trustLevel -= 1
-> END