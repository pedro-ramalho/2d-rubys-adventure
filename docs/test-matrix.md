# Ruby's Adventure — Test Matrix

Systematic pre-release playthrough checklist. Work top to bottom in one sitting. For each failure, note **tier**, **repro steps**, and **observed vs. expected** behavior.

**Triage tiers**

- **S** — Ship-stopper: crash, soft-lock, save corruption, can't complete game
- **A** — Major: confusing UX, missing audio, wrong text/scene, immersion break
- **B** — Minor: awkward timing, small visual glitch, low-impact wrong-feel
- **C** — Cosmetic: single-frame artifact, off-by-1 alignment

**Process**

1. Run the full matrix once without fixing — just note failures.
2. Fix all S and A tier items.
3. Rebuild and run the full matrix again — confirm fixes, catch regressions.
4. Repeat until a full pass surfaces no S/A items. That's ship-ready.

---

## 1. Boot & Main Menu

- [x] **1.1** Launch the build (no prior save). Main menu fades in over ~0.8 s; music plays instantly; Continue is disabled.
- [x] **1.2** Hover each button (New Game, Continue, Options, Quit). All four scale up slightly and brighten.
- [x] **1.3** Move mouse rapidly across buttons. No flicker; scales return correctly when not hovering.
- [x] **1.4** Click each button. Click SFX plays; buttons feel responsive.
- [x] **1.5** Click Quit. Application closes cleanly (build only — won't quit in editor).
- [x] **1.6** Reopen build. Open Options → close. Title position does **not** jump.
- [ ] **1.7** Open Options → tweak each volume slider 0 → max. Volume applies in real time; values persist after close/reopen.
- [x] **1.8** In Options, click Reset to Defaults. All sliders default; rebind labels reset.
- [ ] **1.9** Resize the game window on main menu. UI reflows without clipping or overlap.

## 2. Rebinding (Options menu)

- [x] **2.1** Rebind Dash to Z. Close Options. New binding active in-game.
- [x] **2.2** Rebind Up Primary to T. Save → quit → relaunch. Binding persists.
- [x] **2.3** Click rebind, then press Esc to cancel. Rebind cancels; original preserved; button label unchanged.
- [ ] **2.4** Click rebind → click another rebind without completing first. Note actual behavior (possible bug source).
  - In this case, behavior was weird, because both boxes were in a "Press a key..." state. When I pressed the key (say, X), both boxes were populated with X.
- [ ] **2.5** Rebind two actions to the same key (Dash and Shoot both to E). Note in-game behavior.
  - This was also possible. I was able to rebind two actions to the same key. When this happens, it makes sense to not allow the user to do it. 
- [x] **2.6** Rebind several, then Reset to Defaults. All revert; labels refresh.

## 3. New Game → Level 0 (Martha)

- [x] **3.1** Click New Game. Buttons disable; 3 s overlay fade to black; music fades; Level 0 loads.
- [x] **3.2** Move with WASD and arrow keys. Both work; player faces direction of last input.
- [x] **3.3** Approach Martha. "Press X to talk" prompt appears above her.
- [x] **3.4** Walk away from Martha. Prompt disappears.
- [x] **3.5** Talk to Martha; advance through full dialogue. Each line types out; X advances; final line triggers fruit pickup / quest grant.
- [x] **3.6** Mid-dialogue, walk far from Martha. Dialogue auto-hides.
- [x] **3.7** Talk to Martha, press X rapidly. Each X advances one line; no double-skip; audio doesn't stutter.
- [x] **3.8** Press Tab to open quest tracker. Tracker appears with active quest.
- [x] **3.9** Press Esc/P to pause. Game freezes; pause menu appears; HUD hides; quest tracker hides.
- [x] **3.10** While paused, click Save. "Game saved!" label appears for 2 s, then fades.
- [x] **3.11** Resume from pause. HUD reappears; tracker visibility restored if it was open.

## 4. Combat & dash (Level 1)

- [x] **4.1** Enter Level 1; talk to Frog 1; accept box quest. Dash unlocks; quest tracker shows 0/6.
- [x] **4.2** Try dashing into water tiles. Dash must NOT tunnel into water and trap the player.
- [x] **4.3** Dash through a patrol robot. Player takes no damage during dash (invincibility).
- [x] **4.4** Without dash, run into a patrol robot. Player takes damage; camera shakes; hit SFX plays; HP bar decreases.
- [x] **4.5** Take damage, then take damage again immediately. Second hit ignored (invincibility cooldown).
- [x] **4.6** Collect a box. SFX plays; tracker updates to 1/6.
- [x] **4.7** Collect all 6 boxes. Quest completes; tracker shows completion message; music plays stinger and resumes.
- [x] **4.8** Return to Frog 1; advance to "after" dialogue. Quest concludes; tracker hides; Frog 1 directs to Frog 2.
- [ ] **4.9** Find Frog 2; accept robots quest. Shoot unlocks; projectile launches in facing direction.
  - Patrol Robots hit effect should also be drawn on their center, not at their feet 
- [x] **4.10** Shoot a Vending Machine enemy until it dies. Explosion plays at sprite center (not at feet).
- [x] **4.11** Stand still and shoot in 8 directions (rotate facing first). Each direction launches correctly.
- [ ] **4.12** Take damage to 0 HP. Lose screen fades in ~5 s; defeat sting plays; music fades; same level reloads after ~10 s; respawn at maxHealth.
  - I completed both quests and died in Level 1. Both of my abilities were unlocked. After speaking with Frog 1, they were displaying the "After" section of their dialogue, but I noticed that the "Cargo" game object was inactive, and was reactivated again after talking to the NPC. The same happened to Frog 2's quest. I completed it, and the Patrol Robots were in a "Fixed" state. But after dying and reloading the scene, they were back to wandering and moving around, even though Frog 2 was displaying its "After" dialogue. 

## 5. Scene transition & save integrity

- [x] **5.1** Mid-Level 1, pause → Save → Return to Main Menu. Returns to main menu with smooth transition.
- [ ] **5.2** Click Continue. Loads Level 1 at scene start (not last position); HP matches saved value.
  - Player health is not saved properly. I was at 3 HP, consumed a strawberry, my health increased to 4. Paused the game, saved, and returned to the main menu. After pressing Continue, the level loaded correctly and smoothly, but the previous strawberry I had consumed was still there, and my health had fallen back to 3. 
- [x] **5.3** Continue button still enabled after a continue cycle. Save file persists.
- [x] **5.4** Options → Delete Save → close Options. Continue button immediately disables.
- [x] **5.5** Save mid-fight at 1 HP. Quit → Continue → die. Respawn with maxHealth (death overwrite works).
- [x] **5.6** New Game from main menu; advance via dialogue chain into Level 2. No music carry-over from Level 1 to Level 2.
- [x] **5.7** Continue from a save in Level 2. Loads cleanly; Marshmallow behaves correctly.
- [ ] **5.8** Quit during a scene transition fade (Alt-F4 windowed, or close window). Closes cleanly without errors.

## 6. Level 2 arena

- [x] **6.1** Enter Level 2; talk to Marshmallow; accept quest. Marshmallow walks to exit point; combat music starts; first wave spawns after initial delay.
- [x] **6.2** Pause during a wave spawn. Spawn pauses; resume continues from where it left off; no enemies disappear.
- [x] **6.3** Clear all regular waves. Bonus wave fires after breather; one enemy per spawn point at half spawn interval.
- [x] **6.4** Music behavior during bonus wave matches chosen design (Option A inspector tweak or other).
- [x] **6.5** Clear bonus wave. Marshmallow walks back; epilogue triggers; victory screen fades in; victory sting plays.
- [x] **6.6** After victory screen, return to main menu. Smooth transition; main menu loads with music.
- [x] **6.7** Try to push Marshmallow with the player body. Marshmallow does not move (kinematic Rigidbody).
- [x] **6.8** Shoot Marshmallow. Projectile registers / passes through but Marshmallow doesn't move.

## 7. Audio mixing (separate pass — eyes closed if possible)

- [x] **7.1** Set all sliders to 50%. Play through Level 0 → Level 1 transition. Music balance consistent across scenes.
- [x] **7.2** Stand still on each level for 30 seconds. No silence bugs, no music hitches, no audible loop seams.
- [x] **7.3** Dash + shoot + take damage simultaneously. SFX layer cleanly; nothing drowns out anything else; nothing crackles.
- [x] **7.4** Talk to an NPC; listen to typewriter SFX. Pitch jitter present but not chaotic; volume reasonable; no clipping.
- [x] **7.5** Mute master volume; play 30 s; set master to max. Silence complete at 0; max not painfully loud.
- [x] **7.6** Mute music only, keep SFX. Music silenced; SFX still audible at correct levels.
- [x] **7.7** Trigger victory sting; listen for music conflict. Sting plays cleanly; music fades out without overlap.
- [x] **7.8** Trigger defeat sting (die in a level). Same check for the defeat path.

## 8. UI edge cases

- [x] **8.1** Hold X with dialogue queued. One press = one advance; holding doesn't auto-advance.
- [x] **8.2** Open quest tracker, then advance the active quest. Tracker updates `count/total` in real time without flicker.
- [x] **8.3** Complete a quest while tracker is closed. Tracker pops up with "Quest complete!" message.
- [ ] **8.4** Resize game window mid-dialogue. Text reflows; doesn't truncate weirdly.
- [x] **8.5** Pause while typewriter is mid-line. Typewriter pauses; resume continues the line correctly.
- [x] **8.6** Pause → Return to Main Menu mid-dialogue. Returns cleanly to menu; no dialogue ghost stays.

## 9. Death loop

- [x] **9.1** Die without saving first. Lose screen → reload scene → respawn at maxHealth, scene start position.
  - I would say the death screen plays for too long. It needs to be shortened. 
- [x] **9.2** Die three times in a row in the same scene. Each cycle identical; no state leak (music doesn't pile up; lose screen always at correct opacity).
- [x] **9.3** Die during dialogue. Dialogue clears; lose screen takes over cleanly.
- [x] **9.4** Die on the same frame you complete a quest (final box / final wave enemy). One state wins cleanly; no race conditions.
- [ ] **9.5** Die in Level 2 mid-arena. Lose screen → reload Level 2 → waves do not start (need to re-talk to Marshmallow).
  - This is very buggy. After dying and reloading Level 2, Marshmallow is at the center of the Arena, but plays its "During" dialogue line "This line will never be reached". 

## 10. 60-second first impression

The most important test. **Cold launch the build, set a stopwatch, play for 60 s.**

- [x] **10.1** Game looked polished from the title screen.
- [x] **10.2** First interaction (clicking New Game) felt responsive and satisfying.
- [x] **10.3** Martha's dialogue landed.
- [x] **10.4** Could move and interact intuitively without external instruction.
- [x] **10.5** No moment felt confusing, awkward, or visually rough.

If any of 10.1–10.5 fails, prioritize fixing it above almost anything else. This 60-second window decides whether reviewers keep playing.

---

## Bug log

Use this section to record findings during each pass.

### Pass 1 (date: ____)

| Test # | Tier | Description | Repro | Fixed? |
|---|---|---|---|---|
| | | | | |

### Pass 2 (date: ____)

| Test # | Tier | Description | Repro | Fixed? |
|---|---|---|---|---|
| | | | | |

### Pass 3 (date: ____)

| Test # | Tier | Description | Repro | Fixed? |
|---|---|---|---|---|
| | | | | |
