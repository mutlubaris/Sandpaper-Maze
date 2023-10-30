using System.Collections.Generic;

public static class DataHolder
{
    public static Dictionary<string, Dictionary<Side, ISensor>> SensorInitializerDictionary = new Dictionary<string, Dictionary<Side, ISensor>>();

    private static Dictionary<Side, ISensor> CreateSensorGroup(string instanceId)
    {
        Dictionary<Side, ISensor> SensorInitializerPairs = new Dictionary<Side, ISensor>()
        {
            {Side.FRONT, new StraightSensorData()},
            {Side.BACK, new StraightSensorData() },
            {Side.LEFT, new StraightSensorData() },
            {Side.RIGHT, new StraightSensorData() },

            {Side.FRONTLEFTDIAGONAL, new DiagonalSensorData() },
            {Side.FRONTRIGHTDIAGONAL, new DiagonalSensorData() },
            {Side.BACKLEFTDIAGONAL, new DiagonalSensorData() },
            {Side.BACKRIGHTTDIAGONAL, new DiagonalSensorData() }
        };
        SensorInitializerDictionary.Add(instanceId, SensorInitializerPairs);
        return SensorInitializerPairs;
    }

    public static Dictionary<Side, ISensor> GetSensorGroup(string instanceId)
    {
        if (!SensorInitializerDictionary.ContainsKey(instanceId))
        {
            return CreateSensorGroup(instanceId);
        }
        return SensorInitializerDictionary[instanceId];
    }
}


