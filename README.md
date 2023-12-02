# OxygenEscape
gnext 제 18회 경기게임오디션 in PlayX4

| 작품 이름 | 개발기간 | 엔진 | 개발인원 | 장르 |
| --- | --- | --- | --- | --- |
| Oxygen Escape | 2023.03~2023.04 | Unity / C# | 2명 | 온라인 무중력 술래잡기 |

JSON 으로 서버와 클라이언트간 통신합니다.

> { "type": "exit_room", "room_id": "0914", “user_id”: “hello” }

---

### 클라이언트 주요 코드

```csharp
// 패킷을 읽는 함수
private string ReadPacket()
{
  if (stream.DataAvailable)
  {
    try
    {
      // byte length 먼저 수신
      stream.ReadTimeout = 2000;
      var len_byte = new byte[4];
      stream.Read(len_byte, 0, 4);
      
      int len_int = BitConverter.ToInt32(len_byte,0);
      
      
      // 문자 길이가 300자 이상이면 오류 패킷이므로
      // 예외 처리
      if(len_int > 300)
      {
        clearBuffer();
        return null;
      }
      
      // 바이트 길이만큼 생성 후 수신
      var data = new byte[len_int];
      stream.Read(data, 0, len_int);
      
      // byte to string
      return Encoding.UTF8.GetString(data, 0, len_int);
    }
    catch (Exception e)
    {
      Debug.Log(e.ToString());
    }
  }
  
  return null;
}
```

---

### Client에서 JSON 활용

```csharp
// 패킷을 받아 큐에 넣음
string msg = ReadPacket();

if(msg != null)
{
  // 메세지가 비어있거나 JSON 형태를 지키는지 체크
  if (checkMessage(msg)) 
  {
    try{
    // JSON 해석
    SocketHandler.instance.Handle_Json_Event(msg);
    }
    catch (Exception e)
    {
    Debug.Log("Error : " + e);
    }
  }
}

```

```csharp
public void Handle_Json_Event(string jsonAsString)
{
  try{
    JSON jsonObject = JSON.ParseString(jsonAsString);
    
    string type = jsonObject.GetString("type");
    
    // { "type": "guest_enter", "room_id": "0914", “user_id”: “hello” } 
    // type에 맞게 기타 동작 실행
    if(type == "false")
    {
      return;
    }
    else if(type == "guest_enter")
    {
      // 생략
    }
    else if(type == "guest_exit")
    {
      // 생략
    }
      // 다른 if문 생략
  }
  catch (Exception e){
    Debug.Log(jsonAsString.Length);
    Debug.Log("Error : " + e);
    
    return;
  }
}
```

---

### 서버 주요 코드

```python
try:
  # 클라이언트 바인딩 무한반복
  while True:
    client_socket, addr = server_socket.accept()
    th = threading.Thread(target=binder, args = (client_socket,addr))
    th.start()
except:
  print("Server Except")
finally:
  server_socket.close()
```

```python
# 각 클라이언트별로 할당된 스레드
# binding 함수 핵심 코드
# 자신의 소켓을 user에 담고 있음
while True:
  try:
      data = client_socket.recv(4)
      length = int.from_bytes(data,'big')
      data = client_socket.recv(length)
      data = data.decode()

      if not data:
          break

      # 받은 패킷이 json 인지 검사
      if is_json_string(data):
          handle_json(data,user)

  except ConnectionResetError as e:
      print('Disconnected by ' + addr[0], ':', addr[1])
      break
```

---

**handle_json**에서 **data가 "enter_room"** 형식이면 **userManager.enterRoom(user)** 를 호출합니다. 

user는 각 스레드에 바인딩된 클라이언트 입니다.  따라서 아래와 같이 **user.conn을 통해 서버→클라이언트로 전송**할 수 있습니다.

---

```python
def sendMessageToTarget(self,user,msg):
        if user is None or user.conn is None:
            return

        data = msg.encode()
        length = len(data)

        user.conn.send(length.to_bytes(4, byteorder='big'))
        user.conn.send(data)
```

---

### 바이트 크기 초과 문제 해결

<details>
<summary> 패킷 길이 오류 예시 </summary>
<div markdown="1">
  
![image](https://github.com/Falcon5077/OxygenEscape/assets/32628758/f3d7fc5a-cd98-4f04-8a28-f256e34398aa)

</div>
</details>

<br>

위 링크에서 패킷 길이가 300 이상일 때 손상된 패킷으로 간주하고 처리하지 않도록 예외처리 했지만 클라이언트 쪽에서는 해결이 되는 듯 보였지만 서버 쪽에서는 여전히 말도 안되는 사이즈의 패킷을 보내고 있었습니다.
(마찬가지로 서버에서도 패킷 전송 전 버퍼의 길이가 300 이상이라면 버퍼를 비워줬지만 아래와 같이 나옴)

서버와 클라이언트(A), 클라이언트(B)가 있을 때는 A와 B가 서로 동기화가 잘 됐습니다.
하지만 어쩐 이유인지 클라이언트(C)가 들어왔을 때, (A)는 (B),(C)와 같이 두 개 이상의 클라이언트 에게 동기화 패킷을 전송하는 경우 byte 배열의 크기가 엄청나게 커집니다. 정리하자면 여러개의 클라이언트와 동기화 할 경우 메모리가 터졌다.

결국 해결 했고 해결 과정은 아래와 같습니다.

1. 처음에는 전송할 JSON이 깨져서 문자열 길이를 제대로 인식하지 못하는 줄 알고 JSON을 검사하고 보냈습니다.
-> 해결 안됨
2. 빅 엔디안, 리틀 엔디안이 서버와 클라이언트가 안맞아서 안되는지 의심하였습니다.
-> 만약 인코딩 문제였다면 처음부터 안됐어야하지만 플레이어가 3명일 때부터 오류가 뜬다는건 엔디안 문제가 아님
3. 3명일 때 오류가 난다는 것에 힌트를 얻었습니다. 클라이언트 쪽에서 한번에 여러 패킷을 받느라 (다중 클라이언트 동기화) 패킷이 섞여서 그런줄 알았습니다. (정답)
그래서 클라이언트에서 스레드와 큐를 통해 패킷을 하나 씩 받았습니다.
-> 해결 안됨, 왜? 보내는 곳에서 섞였기 때문에 클라이언트에서 하나 씩 받아도 소용 없음
4. 서버가 클라이언트들한테 메세지를 보낼 때 **스레드 lock**을 이용하여 패킷이 겹치지않게 구현하였습니다.
-> 해결안됨, lock이 걸린 상태에서 특정 클라이언트가 종료되면 소켓이 닫혀서 unlock 되지 못하고 데드락에 빠져버렸습니다.
5. 서버 쪽에서 패킷을 전송할 때 여러 스레드에서 하나의 소켓을 통해 패킷을 전송하는 과정에서 패킷이 섞였던 것입니다.
각 스레드에서 패킷을 전송하기 전에 전송 큐에 삽입하고 메인스레드에서 큐에 저장된 패킷을 꺼내 하나씩 전송하도록 수정하였습니다.

그랬더니 동기화 속도가 느려졌습니다.
클라 (A) -> 서버 -> 클라 (B) 로 동기화를 하는데, 서버를 거치면서 큐에서 잠시 대기하기 때문에 동기화가 느려졌습니다.

각 소켓 별로 전송 큐를 만들었어야 했는데 모든 전송을 하나의 큐로 사용했기 때문에 느려진걸로 생각됩니다. 아쉽게도 해결하지 못한채로 공모전을 마무리 했습니다.


