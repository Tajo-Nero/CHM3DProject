# Reroad

![Development Status](https://img.shields.io/badge/Status-In%20Development-yellow)
![Unity Version](https://img.shields.io/badge/Unity-2021.3%20LTS-blue)
![Platform](https://img.shields.io/badge/Platform-PC-lightgrey)

Unity 기반의 3D 타워 디펜스 게임으로, 플레이어가 직접 지형을 파괴하여 경로를 생성하고 타워를 배치하여 적을 방어하는 게임입니다.
Rabbithole Games에서 개발한 로그라이트 타워 디펜스 게임 을 모작했습니다.
**⚠️ 현재 개발 중인 프로젝트입니다. 일부 기능이 완성되지 않았을 수 있습니다.**

## 목차
- [설치 및 실행](#설치-및-실행)
- [게임 플레이](#게임-플레이)
- [기능](#기능)
- [개발 현황](#개발-현황)
- [시스템 요구사항](#시스템-요구사항)
- [개발 환경](#개발-환경)
- [프로젝트 구조](#프로젝트-구조)
- [기여 방법](#기여-방법)
- [라이선스](#라이선스)

## 설치 및 실행

### 필요 조건
- Unity 2021.3 LTS 이상
- Windows 10 이상

### 설치 방법
1. 저장소를 클론합니다:
   ```bash
   git clone https://github.com/Tajo-Nero/CHM3DProject.git
   ```

2. Unity Hub에서 프로젝트를 추가합니다

3. Unity에서 프로젝트를 열고 Build Settings에서 플랫폼을 선택합니다

4. Play 버튼을 눌러 게임을 실행합니다

## 게임 플레이

### 기본 조작법
- **WASD**: 이동
- **마우스**: 카메라 회전
- **좌클릭**: 타워 배치 / 드릴 모드에서 빠른 이동
- **F5**: 드릴 모드에서 지형 리셋

### 게임 진행
1. **드릴 모드**: 차량을 조작하여 지형을 파괴하고 적의 경로를 생성
2. **타워 배치**: 생성한 경로 주변에 타워를 전략적으로 배치
3. **웨이브 방어**: 총 12개 웨이브의 적을 막아내기

## 기능

### 타워 시스템
- **Cannon Tower**: 기본 포탄 공격
- **Rocket Tower**: 폭발 데미지
- **Laser Tower**: 관통 레이저 공격
- **Buff Tower**: 주변 타워 공격력 강화

### 적 시스템
- 12종류의 다양한 적 (Bee, Cute, Mushroom, Slime, TurtleShell, Elite, Beholder, ChestMonster, Cactus, Boss)
- 웨이브별 난이도 증가

### 특수 시스템
- **지형 파괴**: 실시간 terrain 수정
- **자석 시스템**: 타워 자동 배치 도우미
- **경로 생성**: 플레이어 맞춤형 경로 설계

## 개발 현황

- **개발 상태**: 🚧 개발 중 (In Development)
- **현재 버전**: Alpha 0.1
- **완성도**: 약 70%

### 구현 완료
- ✅ 기본 타워 시스템
- ✅ 적 스폰 및 웨이브 시스템
- ✅ 드릴 모드 지형 파괴
- ✅ UI 시스템

### 개발 예정
- ⏳ 사운드 시스템 완성
- ⏳ 게임 밸런싱
- ⏳ 추가 타워 타입
- ⏳ 최적화 작업

## 시스템 요구사항

### 최소 사양
- OS: Windows 10 64-bit
- Processor: Intel i5-4590 / AMD FX 8350
- Memory: 8 GB RAM
- Graphics: NVIDIA GTX 960 / AMD R9 280
- DirectX: Version 11
- Storage: 2 GB 이상

### 권장 사양
- OS: Windows 11 64-bit
- Processor: Intel i7-7700K / AMD Ryzen 5 2600
- Memory: 16 GB RAM
- Graphics: NVIDIA GTX 1060 / AMD RX 580
- DirectX: Version 12
- Storage: 4 GB 이상

## 개발 환경

- **엔진**: Unity 2021.3 LTS
- **언어**: C#
- **렌더 파이프라인**: Universal Render Pipeline (URP)
- **버전 관리**: Git

### 주요 의존성
- Unity Universal RP
- Unity Audio System
- Unity Physics System

## 프로젝트 구조

```
Assets/
├── MyScripts/
│   ├── Enemy/           # 적 관련 스크립트
│   ├── Tower/           # 타워 시스템
│   │   └── TowerScripts/
│   ├── UI/              # 사용자 인터페이스
│   └── ScriptableObject/ # 게임 데이터
├── Scenes/              # 게임 씬
├── Prefabs/             # 프리팹 에셋
└── Materials/           # 머티리얼 및 텍스처
```

### 핵심 스크립트
- `GameManager.cs`: 전체 게임 흐름 관리
- `TowerBase.cs`: 타워 기본 클래스
- `EnemyData.cs`: 적 데이터 정의
- `PlayerCarMode.cs`: 드릴 모드 제어
- `WaveManager.cs`: 웨이브 시스템 관리

## 기여 방법

1. 이 저장소를 포크합니다
2. 새로운 기능 브랜치를 생성합니다 (`git checkout -b feature/새기능`)
3. 변경사항을 커밋합니다 (`git commit -am '새 기능 추가'`)
4. 브랜치에 푸시합니다 (`git push origin feature/새기능`)
5. Pull Request를 생성합니다

### 코드 스타일
- C# 네이밍 컨벤션 준수
- 주석을 통한 코드 설명
- 클래스별 단일 책임 원칙 적용

## 라이선스

이 프로젝트는 MIT 라이선스 하에 배포됩니다. 자세한 내용은 [LICENSE](LICENSE) 파일을 참조하세요.

## 스크린샷

![게임플레이 스크린샷](screenshots/gameplay.png)
*드릴 모드에서 지형을 파괴하는 모습*

![타워 배치](screenshots/tower-placement.png)
*전략적 타워 배치 화면*

## 개발자

- **개발자**: [Tajo-Nero](https://github.com/Tajo-Nero)
- **프로젝트 기간**: 2024년 ~ 현재
- **개발 툴**: Unity 2021.3 LTS

## 문의

프로젝트에 대한 질문이나 제안사항이 있으시면 이슈를 생성해주세요.

---

⭐ 이 프로젝트가 도움이 되셨다면 스타를 눌러주세요!
