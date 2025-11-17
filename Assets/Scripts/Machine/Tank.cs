using UnityEngine;

public class Tank : BaseMachine
{
    public override void Move(Vector3 _moveDirection)
    {
        base.Move(_moveDirection);

        // // if (_moveDirection.y == 0)
        // // {
        // //     Stop();
        // //     return;
        // // }  

        // Vector3 forward;
        // Vector3 right;

        // if (_gameManager.Settings.simpleMove && !MachineLevelData.isBot)
        // {
        //     forward = levelManager.cinemachineCamera.transform.forward;  //(transform.position - levelManager.cinemachineCamera.transform.position).normalized;
        //     right = levelManager.cinemachineCamera.transform.right;
        // }
        // else
        // {
        //     forward = transform.forward;
        //     right = transform.right;
        // }
        // ;

        // forward.Normalize();
        // right.Normalize();

        // // Vector3 moveDirection = Vector3.zero;
        // // if (_moveDirection.y <= 0.1f && _moveDirection.y >= -0.1f)
        // // {
        // //     moveDirection = Vector3.Cross(transform.up, forward) * _moveDirection.x;
        // // }
        // // else
        // // {
        // //     moveDirection = forward * _moveDirection.y;
        // // }

        // Vector3 moveDirection = (forward * _moveDirection.y + right * _moveDirection.x).normalized;

        // // if (moveDirection != Vector3.zero)
        // // {
        // //     Debug.Log($"moveDirection2 = {moveDirection}");
        // // }

        // Rotate(moveDirection);

        // OnSetDirectionMove(moveDirection);

        // // OnSetNameText(moveDirection.ToString());
        // // transform.Translate(moveDirection * speed * Time.deltaTime);
        // DataBonus bonusSpeed = null;
        // Data.bonuses.TryGetValue(TypeBonus.Speed, out bonusSpeed);
        // var speed = Data.speed + (bonusSpeed != null ? bonusSpeed.value : 0);

        // // kinematic.
        // // rb.MovePosition((Vector3)transform.position + (moveDirection * speed * Time.deltaTime));

        // // dynamic.
        // rb.linearVelocity = moveDirection * (Data.speed + (bonusSpeed != null ? bonusSpeed.value : 0)) * _gameManager.Settings.scaleObjects;
        // // if (rb.linearVelocity.magnitude < 50f)
        // // {
        // //     rb.AddRelativeForce(moveDirection * (100f * Data.speed + (bonusSpeed != null ? bonusSpeed.value : 0)), ForceMode.Impulse); //linearVelocity = moveDirection * (Data.speed + (bonusSpeed != null ? bonusSpeed.value : 0));
        // // }
        // // else
        // // {
        // // }
        // //     Debug.Log($"Magnitude={rb.linearVelocity.magnitude}");

        // //rb.AddForce(moveDirection* (Data.speed * rb.mass + (bonusSpeed != null ? bonusSpeed.value : 0)), ForceMode.Force);

        // // var directionVector = (transform.position - Data.position).normalized;
        // // var movement = new Vector3(directionVector.x, 0f, directionVector.y);

        // // Quaternion lookRotation = Quaternion.LookRotation(movement, Vector3.up);

        // // Debug.Log($"{lookRotation.eulerAngles}, {lookRotation.x}, {lookRotation.y}, {lookRotation.z}");
        // // OnSetAngleBody(lookRotation.eulerAngles.y);

        // // OnSetAngleBody(moveDirection);

        // Data.position = transform.position;

        // for (int i = 0; i < Caterpillars.Count; i++)
        // {
        //     Caterpillars[i].Move();
        // }
        // // for (int i = 0; i < wheels.Count; i++)
        // // {   
        // //     wheels[i].transform.Rotate(Vector3.right, (20f * Data.speed) * Time.deltaTime);
        // // }

        // // Vector3Int posTile = levelManager.mapManager.Map.WorldToCell(transform.position);
        // // GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(posTile);
        // // SetOccupiedNode(node);
    }

    public override void Stop()
    {
        base.Stop();
        
        // // Debug.Log($"stop={rb.linearVelocity.magnitude}");

        // rb.linearVelocity = Vector3.zero;
        // rb.angularVelocity = Vector3.zero;
        // // rb.AddRelativeForce(Vector2.zero);

        // // _textName.text = _speed.ToString();
    }
}
