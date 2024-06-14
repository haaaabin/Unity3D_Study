# 🌴 유니티 스터디 미니 프로젝트 Welcome to Jungle

<img alt="Image" src="https://github.com/JeongJongMun/Welcome_To_Jungle/assets/101979073/a6a3c9ae-5dc3-4278-a4bb-96788ea258c6" style="width: 700px; height: auto; margin: 0 auto;"  />
<br>

## 1. 프로젝트 소개

- 크래프톤 정글 과정 중 스택 공부 기간에 제작한 미니 프로젝트입니다.
- 간단한 로그인으로 멀티 서버에 접속하여 캐릭터를 조작 할 수 있고, 게시판에 글을 남길 수 있습니다.

<br>

## 2. 팀원 구성

<div align="center">

| **최하빈** | **정종문** |
| :------: |  :------: |
| [@haaaabin](https://github.com/haaaabin) | [@JeongJongMun](https://github.com/JeongJongMun) |

</div>

<br>

## 3. 개발 환경

- Client : Unity
- Server & DB : 뒤끝
- 버전 및 이슈관리 : Github
- 협업 툴 : Slack, Notion

<br>

## 4. 프로젝트 구조

```
│─GameManager.cs
│
├─Backend
│   │─ BackendInGame.cs
│   │─ BackendMatch.cs
│   │─ BackendMatchManager.cs
│   │─ BackendNoticeBoard.cs
    └─ BackendServerManager.cs
│
├─System
│   │─ DataParser.cs
│   │─ InputManager.cs
│   │─ Protocol.cs
│   └─ SingletonBase.cs
│
├─UI
│  │
│  ├─InGame
│  │    │─ InGameUI.cs
│  │    └─ NoticeBoard.cs
│  │
│  └─Login
│       └─ LoginUI.cs
│
└─World
    │─ FixedTouchField.cs
    │─ LookCamera.cs
    │─ MainCamera.cs
    │─ Player.cs
    └─ WorldManager.cs
```

<br>

## 5. 역할 분담

### 🍊정종문

- **Level**
    - 3D 월드
- **기능**
    - 뒤끝을 사용한 로그인/회원가입, 매치메이킹, 인게임 서버 접속

<br>
    
### 👻최하빈

- **Level**
    - 로그인/회원가입
- **기능**
    - 조이스틱을 이용한 캐릭터 이동, 터치패드를 이용한 카메라 조작, 게시글 등록 및 삭제, 게시글 상세 확인, 게시글 불러오기

<br>

## 6. 개발 기간

- 전체 개발 기간 : 2024-04-05 ~ 2024-04-18
- UI 구현 : 2024-04-05 ~ 2024-04-09
- 기능 구현 : 2024-04-09 ~ 2024-04-18

<br>

## 7. 참고 예제

- [https://docs.thebackend.io/sdk-docs/backend/match/example-game](https://docs.thebackend.io/sdk-docs/backend/match/example-game)

<br>

## 8. 페이지별 기능

### [초기화면]
- 게임 접속 초기화면으로 로그인과 회원가입이 가능합니다.
    - 로그인 성공/실패, 회원가입 성공/실패 시 팝업창에서 설명이 표시됩니다.
- 로그인 후, 닉네임을 입력하여 매치 메이킹 및 인게임 접속이 가능합니다.
    - 다만, 같은 인게임에 접속하기 위해서 동시에 Play 버튼을 눌러야 합니다.

| 초기화면 |
|----------|
|![image](https://github.com/JeongJongMun/Welcome_To_Jungle/assets/101979073/d29dfc71-85fa-469e-abb5-d3fd67464a84)|
|![image](https://github.com/JeongJongMun/Welcome_To_Jungle/assets/101979073/633cc861-6506-43b6-94d5-407927aad62f)|
|![image](https://github.com/JeongJongMun/Welcome_To_Jungle/assets/101979073/5f266820-3367-4424-83bc-5146de98f2f2)|

### [인게임]
- 캐릭터 조작과 게시판 사용이 가능합니다.
- 카메라는 캐릭터를 따라가고, 캐릭터와 카메라 사이에 장애물이 있을 시, 카메라 위치가 조정됩니다.

| 인게임 |
|----------|
|![image](https://github.com/JeongJongMun/Welcome_To_Jungle/assets/101979073/df766c6d-3073-4173-86a4-85bdb4586d3a)|
|![image](https://github.com/JeongJongMun/Welcome_To_Jungle/assets/101979073/e1ad8f88-d11b-442a-80da-f82fc285e8da)|
|![image](https://github.com/JeongJongMun/Welcome_To_Jungle/assets/101979073/f03c243f-adca-4b32-91b0-c4d2a0ed94aa)|
|![image](https://github.com/JeongJongMun/Welcome_To_Jungle/assets/101979073/de8e1b0c-d925-4713-b1ae-4ed73022b220)|

<br>

## 9. 프로젝트 후기

### 🍊 정종문

뒤끝을 사용하여 클라이언트-서버 구조를 이해할 수 있었다.   
좋은 예제를 통해 학습하는 것이 큰 도움이 되었고, 잘 짜여져 있는 코드를 보고 이해하는 것이 성장에 큰 도움이 된다는 것을 알게 되었다.

<br>

### 👻 최하빈

서버와의 연동을 경험한 적이 없었으나 이번 프로젝트를 기회로 클라이언트와 서버 간의 구조를 이해하고 서버에서 주어지는 API를 활용하여 게시판을 구현할 수 있었다.
이를 통해 클라이언트 뿐만 아니라 서버에 대한 이해의 중요성을 깨닫게 되었다.


