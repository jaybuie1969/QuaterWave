using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This object is used to numerically model and visualize the magnetic field around a quarter-wave vertical antenna
// This is an idealized vizualization and does not actually calculate magnetic field values; instead, it normalizes the field strength from zero to 100 and
// relies on the fact that the magnetic field's magnitude is linearly proportional to amount of current flowing
public class QuarterWaveCurrent : MonoBehaviour {
    // Allow for the look of each point in this vsualization to be defined in Unity
    [SerializeField] Transform pointPrefab;

    // Allow for the wavelength of the signal's carrier wave to be defined in Unity, limiting its range to the desired limits
    // The carrier wave's wavelength is defined in terms of "ticks" of the model's clock
    // For example, if wavelength is 400 ticks, the carrier wave's value will be a sine wave that repeats every 400 ticks
    [SerializeField] [Range(10, 800)] int wavelength = 400;

    // Set the other parameters used by this object
    int length = 100;
    int pointsAround = 100;
    float rMax = 100f;

    Transform[] points;
    AntennaElement[] antennaElements;

    int t = 0;
    int lengthMinusOne = -1;

	// This method handles the activities done when this object is instantiated
	void Awake() {
        // Create the initial set of points objects
        points = new Transform[pointsAround * length];
        for (int i=0; i<points.Length; i++) {
            points[i] = Instantiate(pointPrefab);
            points[i].localScale = Vector3.one;
            points[i].SetParent(transform, false);
        }

        // Create an array of antenna elements the size of this antenna's length
        // NOTE:  The antenna's length is defined in terms of "ticks" of the model's clock
        // For example, if an antenna has a length of 100 ticks, current entering the antenna's base at tick #N will reach the end of the antenna a tick #(N + 100)
        antennaElements = new AntennaElement[length];

        // length - 1 is used quite a bit, so set a variable for quick access to that value
        lengthMinusOne = length - 1;
    }

    // This method is called at regular time intervals and is used to re-generate the "terrain" i.e. the magnetic field "envelope" surrounding the anntenna
    void Update() {
        // Advance the current by one clock cycle (the variable t)
        AdvanceCurrent();

        // Iterate through each point along the antenna and calculate the radial points coordinates for each element

        // First, along the length of the antenna
        for (int i=0; i<length; i++) {
            // Next, iterate around the longitude of this point on the antenna
            for (int j=0; j<pointsAround; j++) {
                points[(i * length) + j].localPosition = AmplitudePoint(i, j);
            }
        }
    }

    // This method calculates the x, y and z positions of the given point
    // The incoming arguments are:
    //      The point's linear position along the antenna's raidial axis (integer ranging from 0 to its length)
    //      The point's radial longitudinal position around the antenna (integer ranging from 0 to the number of points around each point along the axis
    Vector3 AmplitudePoint(int x, int rotator) {
        // Initialize the postion Vector3 object that will be returned
        Vector3 p;

        // Get the current cuurent (pun fully intended) value for this antenna element
        // A positive value indicates current flowing toward the end of the antenna
        // A negative value indicates current flowing toward the base of the antenna
        float i = antennaElements[x].GetI();

        // Calculate the position's X coordinate in the virtual environment as the cosine part of the ring around the antenna
        p.x = rMax * i * Mathf.Cos(2 * Mathf.PI * (float)rotator / (float)pointsAround);

        // Use the incoming x as the position's Y coordinate in the virtual environment
        p.y = x;

        // Calculate the position's Z coordinate in the virtual environment as the sine part of the ring around the antenna
        p.z = rMax * i * Mathf.Sin(2 * Mathf.PI * (float)rotator / (float)pointsAround);

        return p;
    }

    // This method advances the current running through the antenna's elements by one clock cycle (the variable t)
    void AdvanceCurrent () {
        // "Tick"
        // Iterate forward through antennaElements and move the "backward" current values one element down the array
        for (int counter = 0; counter < lengthMinusOne; counter++) {
            antennaElements[counter].iBackward = antennaElements[counter + 1].iBackward;
        }

        // For the last element in antennaElements, move its "forward" current value to its "backward" current field
        antennaElements[lengthMinusOne].iBackward = -antennaElements[lengthMinusOne].iForward;

        // "Tock"
        // Iterate backward through antennaElements and move the "forward" current values one element up the array
        for (int counter = lengthMinusOne; counter > 0; counter--) {
            antennaElements[counter].iForward = antennaElements[counter - 1].iForward;
        }

        // Calculate the new current value and place it at the beginning of antennaElements as its "forward" current
        antennaElements[0].iForward = Mathf.Sin((float)t * 2f * Mathf.PI / (float)wavelength);

        // Increment t for the next trip through this method
        t++;
	}

    // This internal class object represents a single element of the antenna
    // Current flows in either direction through it, and the two currents add together
    struct AntennaElement {
        public float iForward;
        public float iBackward;

        // This is the constructor that instantiates an AntennaElement object
        AntennaElement(float _iForward=0f, float _iBackward=0f) {
            iForward = _iForward;
            iBackward = _iBackward;
        }

        // This method returns the total current flowing through the antenna element
        public float GetI() {
            return iForward + iBackward;
        }
    }
}
