# Reroad


Unity 기반의 3D 타워 디펜스 게임으로, 플레이어가 직접 지형을 파괴하여 경로를 생성하고 타워를 배치하여 적을 방어하는 게임입니다.
Rabbithole Games에서 개발한 로그라이트 타워 디펜스 게임 을 모작했습니다.

## 목차
- [게임 플레이](#게임-플레이)
- [기능](#기능)
- [개발 환경](#개발-환경)
- [프로젝트 구조](#프로젝트-구조)


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
- **경로 생성**: 플레이어 맞춤형 경로 설계

### 구현 완료
- 기본 타워 시스템
- 적 스폰 및 웨이브 시스템
- 드릴 모드 지형 파괴
- UI 시스템

## 개발 환경

- **엔진**: Unity 2021.3 LTS
- **언어**: C#
- **버전 관리**: Git

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


## 개발자

- **개발자**: [Tajo-Nero](https://github.com/Tajo-Nero)
- **프로젝트 기간**: 2025년 2월 14일 ~ 현재
- **개발 툴**: Unity 2021.3 LTS

