//Shrub Quest


VAR ballCount = 0
VAR trustLvl = 0
VAR trustGrade = 0
VAR finished = false
VAR accepted = false

-> decide

=== decide ===
{trustGrade == 0:
    alias: Nothing to say -> END
    - else:
        {finished:
            -> final
            -else:
                {accepted:
                    -> checkQuest
                    -else:
                    -> askQuest
                }
        }
}


=== askQuest ===
alias: Hello, kind person. Can you help me?
    * Sure, what is it?
        ~trustLvl += 0.5
        #quest
        alias: Oh thank goodness. By the way I'm Shrub.
        you: Nice to meet you Shrub.
        npc: Thank you. I need you to pick me up 3 black balls.
        you: Sure, wait for me.
        -> END
    * No, I'm busy
        ~trustLvl -= 0.5
        alias: Too bad
        -> END

=== checkQuest ===
npc: Hello, kind person. Let me see your balls.
{ballCount == 3:
            -> final
            - else:
                npc: I need {3 - ballCount} more, kind person.
                -> END
        }

=== final ===
npc: Thank you so much, kind person.
-> END