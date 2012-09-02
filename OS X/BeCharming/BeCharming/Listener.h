//
//  Listener.h
//  BeCharming
//
//  Created by Tomas McGuinness on 02/09/2012.
//  Copyright (c) 2012 tomasmcguinness.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "GCDAsyncUdpSocket.h"

@interface Listener : NSObject

@property (nonatomic, retain) GCDAsyncUdpSocket *socket;
@property (nonatomic, retain) GCDAsyncUdpSocket *serverSocket;

- (void)start;
- (void)stop;

@end
