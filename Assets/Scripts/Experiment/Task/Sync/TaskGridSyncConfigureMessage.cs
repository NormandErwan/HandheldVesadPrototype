using DevicesSyncUnity.Messages;
using System;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task.Sync
{
  public class TaskGridSyncConfigureMessage : DevicesSyncMessage
  {
    // Constructors and destructor

    public TaskGridSyncConfigureMessage(TaskGrid taskGrid, Action sendToServer) : base()
    {
      TaskGrid = taskGrid;
      SendToServer = sendToServer;

      taskGrid.ConfigureSync += TaskGrid_ConfigureSync;
    }

    public TaskGridSyncConfigureMessage() : base()
    {
    }

    ~TaskGridSyncConfigureMessage()
    {
      TaskGrid.ConfigureSync -= TaskGrid_ConfigureSync;
    }

    // Properties

    public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }
    public override short MessageType { get { return MasterThesis.Experiment.MessageType.GridConfigure; } }

    protected TaskGrid TaskGrid { get; private set; }
    protected Action SendToServer { get; private set; }

    // Variables

    public int senderConnectionId;

    public int columnsNumber, rowsNumber, itemsPerContainer, incorrectContainersNumber;
    public int[] itemValues;

    // Methods

    public void ConfigureGrid(TaskGrid taskGrid)
    {
      var containers = new GridGenerator.Container[rowsNumber, columnsNumber];
      IterateContainers(rowsNumber, columnsNumber, itemsPerContainer, (row, col, itemIndex, valueIndex) =>
      {
        if (itemIndex == 0)
        {
          containers[row, col].items = new int[itemsPerContainer];
        }
        containers[row, col].items[itemIndex] = itemValues[valueIndex];
      });

      var gridGenerator = new GridGenerator(containers, incorrectContainersNumber);
      taskGrid.SetConfiguration(gridGenerator);
    }

    protected void TaskGrid_ConfigureSync(GridGenerator gridGenerator)
    {
      columnsNumber = gridGenerator.ColumnsNumber;
      rowsNumber = gridGenerator.RowsNumber;
      itemsPerContainer = gridGenerator.ItemsPerContainer;
      incorrectContainersNumber = gridGenerator.IncorrectContainersNumber;

      itemValues = new int[rowsNumber * columnsNumber * itemsPerContainer];
      IterateContainers(rowsNumber, columnsNumber, itemsPerContainer, (row, col, itemIndex, valueIndex) =>
      {
        itemValues[valueIndex] = gridGenerator.Containers[row, col].items[itemIndex];
      });

      SendToServer();
    }

    protected void IterateContainers(int rowsNumber, int columnsNumber, int itemsPerContainer, Action<int, int, int, int> onEachItem)
    {
      for (int row = 0; row < rowsNumber; ++row)
      {
        for (int col = 0; col < columnsNumber; ++col)
        {
          for (int itemIndex = 0; itemIndex < itemsPerContainer; ++itemIndex)
          {
            var valueIndex = row * columnsNumber * itemsPerContainer + col * itemsPerContainer + itemIndex;
            onEachItem(row, col, itemIndex, valueIndex);
          }
        }
      }
    }
  }
}
