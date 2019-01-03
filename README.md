# QuaterWave
Visualize the magnetic field around a quarter-wave dipole antenna
Developed using Unity engine and C#

This is a prototype proof-of-concept I did back in May of 2018 where I was tryng to see if I could use Unity to create and manipulate a dynamic 3-dimensional model completely through code.  This was not for work or for a class, but just for the heck of it.  Yes, I am a nerd, I know.

This visualization can be used as a demonstration of the efficiency of resonant versus non-resonant antenna length in an RF communication system.

At this point, this is a purely qualitative visualization.  There are no quantities associated with it.  Those may come later.

** UNITY AND C# **
This project was built and is designed to run using the Unity engine's 3-D virtual environment, but the engine itself serves as little more than a backdrop against which the visualization unfolds.  Other than making some fields configurable via Unity's IDE, the code's "game object" is simply placed into the virtual environment and allowed to run.

Since my undergraduate is in computer engineering and I've always had an interest in RF and signals, I gravitated toward something that is useful, complicated, difficult to visualize in static images and (most importantly) gave me fits when I was learning it in class.

** DESCRIPTION OF QUARTER-WAVE DIPOLE **
A quater-wave dipole antenna consists of one half of a half-wave dipole.  It consists of a simple linear conducting path down which electricity travels.  The best visual example is the traditional AM/FM radio antenna on cars and walkie-talkies.  This was used because it is a simple and easy first test.

The signal's electrical current comes from the transmitter and enters the antenna at one end.  It travels to the end of the antenna and then returns back along the path from which it came, gonig back to the transmitter.  In this simplified example, all reactive attenuation and interference have been ignored.

The total current going through any small piece of the antenna at any time is the sum of the currents going in each direction.  Since the strength of the magnetic field is linearly proporitional to the current, the current value serves as a stand-in for the actual magnetic field and is then normalized between 0 and 100.


** SIMULATION EXECUTION **
The simulation starts at t = 0.  For all t < 0, i == 0.  In other words, no current was flowing before the simulation began.  The system is not in steady-state.  Conceptually, this is the same as what happens when you turn on the transformor or "key the mike."

Once the transmitter is running (i.e. t >= 0), the current flowing to the transmitter is defined as i = RadiusMax * sin(2 * PI * t / WaveLength).

* RadiusMax is defined as 100 in this simulation (i.e. normalied between 0 and 100)
* WaveLength is modifiable within the Unity IDE, but is initialized to 400 (there is no official unit for wavelength, for lack of a better term, the unit could be called "antenna units")

The simulation defines the antenna as a straight line of 100 individual elements (the aforementioned "antenna unit").  Each element contains two current attributes -- one for primary current coming from the transmitter, and the other for reflected current coming from the end of the antenna.

Since this demonstration is for a quater-wave antenna, the simulatin is initialized to use a resonant frequency.  Since the antenna is 100 units long, a resonant wave will have a wavelength four times as long as the antenna (hence the name "quarter-wave"), or 400 units.

To render the visualization, the object iterates over the antenna's elements and then creates a ring of 100 Unity "prefab" objects arond it.  The radius of this ring changes depending on the amount of electrical current flowing through the element.  By default, Unity's cube prefab is used for each part of the ring, but this can be changed within the Unit IDE.

Each time the simulation "ticks" forward, t is incremented by one, each antenna elements' i values are re-calculated, and then the rings' radii are re-calculated using the new i values.
