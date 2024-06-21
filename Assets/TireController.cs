using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;


public class TireController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField distanceInput;
    [SerializeField]
    private TMP_InputField durationInput;
    [SerializeField]
    private TMP_Dropdown roadInput;
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private GameObject[] tires;
    [SerializeField]
    private Material wornTireMat;

    [SerializeField]
    private TMP_Text SpeedText;
    [SerializeField]
    private TMP_Text HealthText;
    [SerializeField]
    private TMP_Text DamageText;

    [SerializeField]
    private float TireLoss = 0.0f;
    [SerializeField] 
    private float TireStatus = 100;
    [SerializeField] 
    private float Threshold = 40;

    private float tireRotationSpeed = 0.0f;
    private List<CSVRow> csvData = new List<CSVRow>();
    
    public class CSVRow
    {
        public string stringValue;
        public float tireLossPercentage;

    }

    void Start()
    {
        // Add listener to the button
        submitButton.onClick.AddListener(HandleSubmit);
        // Read the CSV file
        ReadCSVFile("Book1.csv"); // Replace "your_csv_file.csv" with your CSV file name

        // Add options to the dropdown
        foreach (var row in csvData)
        {
            roadInput.options.Add(new TMP_Dropdown.OptionData(row.stringValue));
        }

    }

    void Update()
    {
        // Rotate tires
        tires[0].transform.Rotate(Time.deltaTime * tireRotationSpeed * Vector3.left);
        tires[1].transform.Rotate(Time.deltaTime * tireRotationSpeed * Vector3.left);
        tires[2].transform.Rotate(Time.deltaTime * tireRotationSpeed * Vector3.right);
        tires[3].transform.Rotate(Time.deltaTime * tireRotationSpeed * Vector3.right);
        //foreach (GameObject tire in tires)
        //{
        //    tire.transform.Rotate(Time.deltaTime * tireRotationSpeed * Vector3.left);
        //}
    }


    public void HandleSubmit()
    {
        if (distanceInput.text == "" || durationInput.text == null)
        {
            Debug.LogError("Input Field or Speed Text not assigned.");
            return;
        }

        // Parse the text from the input fields
        if (int.TryParse(distanceInput.text, out int distance) && int.TryParse(durationInput.text, out int duration))
        {  
            string roadType = roadInput.options[roadInput.value].text;
            CSVRow foundRow = SearchCSVRow(roadType);

            if (foundRow != null)
            {
                float speed = (float)(distance / (duration * 0.0166));
                tireRotationSpeed = speed * 10;
                SpeedText.text = "Average Speed: " + speed.ToString("F2") + " km/h";

                float rpm = distance * 1000; // considering car tire circumference of 1m
                TireLoss = rpm * foundRow.tireLossPercentage;
                TireStatus -= TireLoss;
                Debug.Log(TireLoss);
                HealthText.text = "Current Health: " + TireStatus.ToString("F2") + " %";
                DamageText.text = "Last Damage: " + TireLoss.ToString("F2") + " %";


                UpdateTireColor();
            }
        }
        else
        {           
            Debug.LogError("Invalid input. Please enter valid numbers.");
        }

    }

    // Function to read the CSV file
    private void ReadCSVFile(string filePath)
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read all lines from the CSV file
            string[] lines = File.ReadAllLines(filePath);

            // Parse each line and store data
            foreach (string line in lines) // Skip the header row
            {
                string[] fields = line.Split(',');
                if (fields.Length == 2) // Assuming the CSV has 2 columns (stringValue, tireLossPercentage)
                {
                    csvData.Add(new CSVRow
                    {
                        stringValue = fields[0],
                        tireLossPercentage = float.Parse(fields[1])
                    });
                }
                else
                {
                    Debug.LogError($"Invalid CSV data: {line}");
                }
            }
        }
        else
        {
            Debug.LogError($"CSV file not found at path: {filePath}");
        }
    }

    // Function to search for a row in the CSV data based on stringValue
    private CSVRow SearchCSVRow(string searchString)
    {
        return csvData.FirstOrDefault(row => row.stringValue == searchString);
    }

    private void UpdateTireColor()
    {
        if (TireStatus < 0) { 
            tireRotationSpeed = 0;
            submitButton.onClick.RemoveAllListeners();
            return; 
        }

        // Check if the status is less than the threshold
        if (TireStatus < Threshold)
        {
            Debug.Log("Tire colors will be changed");

            // Change the color of clockwise tires to red
            foreach (GameObject tire in tires)
            {
                Transform temp = tire.transform.Find("Tire Base");
                if (temp.TryGetComponent<Renderer>(out var renderer))
                {
                    renderer.material = wornTireMat;
                }
            }
        }
    }
}
