VERSION 2.2.0.4 (July 10, 2013)

NEW:	Emergency Fuel Service! Call to GET-555-FUEL (438-555-3835) or press K (if phone
		number checks are not working on yours) to call a emergency fuel bouser, which will
		enter your scene when you ran out of all options (no fuel bottles and couldn't reach
		to a fueling station in time).
NEW:	Now you can set how much fuel bottles player can carry as maximum (MAXFUELBOTTLES).
NEW:	Now you can set how much free fuel bottles should be added to the player's
		inventory when the game loaded (FREEBOTTLES).
NEW:	Now you can set how much should player spend to get one fuel bottle (FUELBOTTLECOST).
NEW:	Now you can set how much should player cost to get emergency fuel service call (SERVICECOST).
NEW:	Now you can set which key should press to call emergency fuel service if phone number
		checks are not working (calling won't work) by SERVICEKEY.
NEW:	Added text notification configurations for: EMERGENCYCALLTEXT, EMERGENCYONWAYTEXT,
		EMERGENCYAGENTTEXT, EMERGENCYDONETEXT.
CHANGE:	Optimized FuelScript_Tick() function to be calculated a little more faster.
CHANGE: Added back Play() function to play embedded sound when player enters to the reserved
		fuel on a vehicle.
FIX:	Fixed few percentage and amount calculators to use String.Format() which is a
		little more faster in overall performance (Solution by Pedro).
FIX:	Beta versions doesn't say that it's a beta properly.
FIX:	Crash when player gets back to vehicle after refueling his vehicle with a fuel bottle.
FIX:	Crash when player luckily gets to a fueling station even when no fuel (using the speed
		he gained in the vehicle).
FIX:	Fixed few comments which were commented with wrongful meaning.
FIX:	Fixed script crash when player taking too much time to get back in vehicle
		after injecting a fuel bottle (known when player runs on lower FPS than game
		playable FPS, it crashes as player takes too much time to get back on vehicle).
FIX:	Fixed unwanted double "if" checks which can be collapsed into one check.
FIX:	Dozen more minor bugs has been fixed.
NOTE:	Fully commented newly added features and functions on the source code.

VERSION 2.2.0.3 (July 07, 2013)

CHANGE:	Now Niko is not getting off from Helicopters and Boats to inject fuel bottles.
		It also means there's no animation for those type of vehicles, just fuel
		bottle deduction and increment of fuel level.
CHANGE: Changed configuration setting: CLASSICGAUGEWIDTH to W, making the dashboard
		position parameters easy as X, Y and W (alias for width).
CHANGE:	Changed configuration setting: MISSIONVEHICLESDRAIN to MVDRAIN, making it short
		while the description is in there to user reference.
FIX:	Static fuel percentage position if user decreased fuel meter width, now it's
		dynamic and will automatically adjust to where fuel meter ends.
FIX:	Issue with Bus, when using a fuel bottle script crashes. Temporary fixed by
		not letting Niko get off the bus.
FIX:	Script waiting times are now automatically adjust according to player's position
		and state, and script will properly wait for next events without a crash.
FIX:	Fixed few errors that caused to crash the script when entering a new vehicle.
RMVD:	Time Cycle change (black & white effect) when entered to reserved fuel.
NOTE:	Started beta and nightly builds.
NOTE: 	Fully commented, properly structured source code is now available at Google Code
		Project Hosting (https://code.google.com/p/realistic-fuel-mod).

VERSION 2.2.0.2 (July 02, 2013)

ADD:	Now you can toggle whether mission vehicles will drain fuel or not by
        adjusting settings at [MISC] -> MISSIONVEHICLESDRAIN.
ADD:	When you entered to reserved fuel, screen will turn black & white.
        By refueling or using a fuel bottle turns this state back to normal.
        Can be turned on or off at the config file [MISC] -> EFFECTS.
CHANGE:	Now you can turn on or off any text notification showing on the
        screen using the config file [TEXTS] area. Detailed information are there.
FIX:	Fixed HELPKEY to BOTTLEUSEKEY, an BOTTLEKEY to BOTTLEBUYKEY making
        the configurations more clearly.
FIX:	Fixed vehicle repair animation delay (decreased to 2 seconds instead
        of 3 second waiting time).
FIX:	Ticking function optimizations, hopefully a little more performence.
FIX:	Fixed dozen more minor bugs to get a better performence.

VERSION 2.2.0.1 (June 30, 2013)

ADD:	Fuel bottles! If you ran out of fuel before you can reach a fueling stations,
        you can use fuel bottles to get fill your tank around 1/3. You can have maximum
        of 5 fuel bottles. You gets 3 free fuel bottles when the game loads. You can buy
        more fuel bottles at fueling stations with B button for $129.99 per bottle.
ADD: 	Fuel percentage indicator! Now you can see how much fuel you have left in your
        vehicle, at right side of the fuel meter. Such as 57% means you have 57% fuel
        left in your vehicle currently.
ADD: 	Fuel bottles indicator! Now you can see how much fuel bottles you have in your
        inventory. Such as 3/5 means you have 3 fuel bottles out of 5, and you can buy
        2 more bottles at fueling station to make it 5 total bottles.
ADD: 	Everytime you enters a vehicle a note will be shown in top left corner of the
        screen informing how much fuel this vehicle have and how much fuel bottles you have.
ADD: 	100% onscreen notifications and guides! Everything will be drawn onscreen (top left)
        such as when you enters reserved fuel on your vehicle, when you ran out of fuel
        (what to do, with keyboard keys to use actions), when you using a fuel bottle, and more.
ADD: 	Fuel in mission required vehicles will not be drained until the mission is ended.
        Only free roaming vehicles fuel will be affected to the draining!
ADD: 	Fuel bottles buy (to buy fuel bottles, default B) and use (to use fuel bottles if
        you ran out of fuel, default U) keys.
ADD: 	Added 2 more command script functions: GetCurrentFuelPercentage() which outputs the
        current fuel as a percentage of the tank's maximum capacity, GetCurrentFuelBottles()
        which outputs how many fuel bottles player has.
ADD: 	Added welcome messages to fueling stations which shows their prices and guides
        to purchase fuel.
CHANGE:	When you ran out of fuel, you vehicle will pop smoke from the engine as the engine
        fails. It will be fine when you refueled or used a fuel bottle.
CHANGE:	When you ran out of fuel, you needs to stop the vehicle and make engine idle before
        performing any actions.
CHANGE:	Script is now logging almost every actions (if user set LOG to true on config file),
        not just errors.
CHANGE:	Cars and bikes now fills the tank with 5 units per second, while helicopters and
        boats fills the tank with 10 units per second.
FIX:	Suspeciously fixed CLASSICGUAGEWITH to CLASSICGUAGEWIDTH (mispelled WITH to WIDTH).
RMVD: 	The phonecall animation! Which was if you ran out of fuel, calling to a friend or
        someone will give you little fuel to your tank back. I though that was little unrealistic
        and I removed that and implemented this fuel bottle thingy and fuel bottle inject
        animation instead.
RMVD: 	Save car functionality which was also not present in the new version of Ultimate Fuel
        Script. In Ultimate Fuel Script v2.1.0.0's source, it was already removed by "pedro2555".